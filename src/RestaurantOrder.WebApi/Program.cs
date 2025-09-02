using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantOrder.WebApi.Core.Identity;
using RestaurantOrder.WebApi.Core.Interfaces;
using RestaurantOrder.WebApi.Infrastructure;
using RestaurantOrder.WebApi.Infrastructure.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Configure Identity
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configure Authentication
var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? "your-secret-key-here-must-be-at-least-256-bits";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "restaurant-order-api",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "restaurant-order-client",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Auth:Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"] ?? "";
    options.CallbackPath = "/signin-google";
});

builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Configure Controllers
builder.Services.AddControllers();

// Configure API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Restaurant Order API", 
        Version = "v1",
        Description = "A complete restaurant order management API"
    });
    
    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Order API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Initialize Database and Seed Data
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Seed roles
        string[] roles = { "Manager", "Server", "Cashier" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new AppRole(role));
            }
        }
        
        // Seed admin user
        var adminEmail = "admin@restaurant.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "System Administrator"
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Manager");
                Log.Information("Admin user created successfully");
            }
        }
        
        // Seed sample menu data
        await SeedMenuDataAsync(context);
        
        Log.Information("Database initialization completed");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while initializing the database");
    }
}

Log.Information("Starting Restaurant Order API");

app.Run();

// Helper method to seed menu data
static async Task SeedMenuDataAsync(AppDbContext context)
{
    if (await context.MenuCategories.AnyAsync())
        return; // Already seeded

    var categories = new[]
    {
        new { Name = "Appetizers", SortOrder = 1 },
        new { Name = "Main Courses", SortOrder = 2 },
        new { Name = "Desserts", SortOrder = 3 },
        new { Name = "Beverages", SortOrder = 4 }
    };

    var menuCategories = new List<RestaurantOrder.WebApi.Core.Entities.MenuCategory>();
    
    foreach (var cat in categories)
    {
        var category = new RestaurantOrder.WebApi.Core.Entities.MenuCategory
        {
            Name = cat.Name,
            SortOrder = cat.SortOrder
        };
        menuCategories.Add(category);
        context.MenuCategories.Add(category);
    }

    await context.SaveChangesAsync();

    // Seed menu items
    var menuItems = new[]
    {
        new { Code = "APP001", Name = "Caesar Salad", CategoryName = "Appetizers", Price = 8.99m },
        new { Code = "APP002", Name = "Chicken Wings", CategoryName = "Appetizers", Price = 12.99m },
        new { Code = "MAIN001", Name = "Grilled Salmon", CategoryName = "Main Courses", Price = 24.99m },
        new { Code = "MAIN002", Name = "Beef Steak", CategoryName = "Main Courses", Price = 29.99m },
        new { Code = "MAIN003", Name = "Pasta Carbonara", CategoryName = "Main Courses", Price = 16.99m },
        new { Code = "DES001", Name = "Chocolate Cake", CategoryName = "Desserts", Price = 6.99m },
        new { Code = "DES002", Name = "Ice Cream", CategoryName = "Desserts", Price = 4.99m },
        new { Code = "BEV001", Name = "Coffee", CategoryName = "Beverages", Price = 3.99m },
        new { Code = "BEV002", Name = "Soft Drink", CategoryName = "Beverages", Price = 2.99m }
    };

    var today = DateOnly.FromDateTime(DateTime.Today);

    foreach (var item in menuItems)
    {
        var category = menuCategories.First(c => c.Name == item.CategoryName);
        
        var menuItem = new RestaurantOrder.WebApi.Core.Entities.MenuItem
        {
            Code = item.Code,
            Name = item.Name,
            CategoryId = category.Id
        };
        
        context.MenuItems.Add(menuItem);
        await context.SaveChangesAsync();

        var menuPrice = new RestaurantOrder.WebApi.Core.Entities.MenuPrice
        {
            MenuItemId = menuItem.Id,
            EffectiveDate = today,
            Price = item.Price,
            Currency = "TWD"
        };
        
        context.MenuPrices.Add(menuPrice);
    }

    await context.SaveChangesAsync();
    Log.Information("Sample menu data seeded successfully");
}

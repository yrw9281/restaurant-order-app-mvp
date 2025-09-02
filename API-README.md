# Restaurant Order Management API

這是一個完整的餐廳點餐管理系統 WebAPI，基於 .NET 9 開發，使用 PostgreSQL 作為資料庫。

## 🏗️ 系統架構

- **平台**: .NET 9 Web API
- **資料庫**: PostgreSQL 
- **ORM**: Entity Framework Core
- **身份驗證**: ASP.NET Core Identity + JWT + Google OIDC
- **架構**: 3層架構 (Api / Core / Infrastructure)
- **日誌**: Serilog
- **文檔**: Swagger/OpenAPI

## 🔥 主要功能

### 選單管理
- 選單分類 CRUD
- 選單項目 CRUD  
- 每日價格管理
- 今日選單 API (供前端使用)

### 訂單管理
- 訂單生命週期：草稿 → 提交 → 確認 → 付款
- 訂單項目管理
- 內用/外帶訂單類型
- 自動計算總金額 (小計、稅金、服務費)

### 身份驗證與授權
- 使用者註冊/登入
- JWT Token 驗證
- 角色權限控制：Manager, Server, Cashier
- Google OAuth 登入支援

### 支付管理
- 支付記錄
- 多種支付方式
- 支付確認流程

### 報表功能
- 每日銷售報表
- 熱門商品統計

### 稽核日誌
- 所有重要操作的審計記錄
- 操作者追蹤

## 🚀 快速開始

### 前置需求
- .NET 9 SDK
- PostgreSQL 資料庫
- (可選) Docker & Docker Compose

### 本地開發

1. **克隆專案**
   ```bash
   git clone <repository-url>
   cd restaurant-order
   ```

2. **設定資料庫連線**
   
   編輯 `src/RestaurantOrder.WebApi/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Host=localhost;Port=5432;Database=mydb;Username=admin;Password=secret"
     }
   }
   ```

3. **安裝相依套件**
   ```bash
   cd src/RestaurantOrder.WebApi
   dotnet restore
   ```

4. **執行應用程式**
   ```bash
   dotnet run
   ```

5. **開啟 Swagger UI**
   
   瀏覽器開啟：`http://localhost:5248/swagger`

### 使用 Docker

```bash
# 啟動所有服務 (PostgreSQL + API)
docker-compose up -d

# 查看日誌
docker-compose logs -f

# 停止服務
docker-compose down
```

## 🔐 預設帳號

系統啟動時會自動建立管理員帳號：

- **Email**: `admin@restaurant.com`
- **Password**: `Admin123!`
- **Role**: Manager

## 📖 API 使用指南

### 身份驗證

1. **註冊新用戶**
   ```bash
   POST /api/auth/register
   {
     "email": "user@example.com",
     "password": "Password123!",
     "displayName": "User Name"
   }
   ```

2. **登入取得 Token**
   ```bash
   POST /api/auth/login
   {
     "email": "admin@restaurant.com", 
     "password": "Admin123!"
   }
   ```

3. **使用 Bearer Token**
   
   在 Header 中加入：`Authorization: Bearer <your-jwt-token>`

### 選單管理

```bash
# 取得今日選單 (公開 API)
GET /api/menu/today

# 取得所有分類
GET /api/menu/categories

# 建立選單項目 (需要 Manager 權限)
POST /api/menu/items
{
  "categoryId": "category-uuid",
  "code": "ITEM001", 
  "name": "商品名稱"
}

# 設定今日價格 (需要 Manager 權限)
POST /api/menu/items/{itemId}/prices
{
  "menuItemId": "item-uuid",
  "effectiveDate": "2025-09-01",
  "price": 299.00,
  "currency": "TWD"
}
```

### 訂單管理

```bash
# 建立訂單
POST /api/orders
{
  "type": "DineIn",
  "partySize": 4,
  "tableNo": "T001"
}

# 新增訂單項目
POST /api/orders/{orderId}/items
{
  "menuItemId": "item-uuid",
  "quantity": 2,
  "notes": "無洋蔥"
}

# 提交訂單
POST /api/orders/{orderId}/submit

# 確認訂單 (需要 Cashier 或 Manager 權限)
POST /api/orders/{orderId}/confirm

# 訂單付款 (需要 Cashier 或 Manager 權限)
POST /api/orders/{orderId}/pay
{
  "amount": 598.00,
  "method": "Cash"
}
```

## 🎭 角色權限

| 角色 | 權限 |
|------|------|
| **Manager** | 所有權限 (選單管理、訂單管理、用戶管理、報表) |
| **Cashier** | 訂單確認、收款、報表查看 |
| **Server** | 建立訂單、修改訂單、提交訂單 |

## 🔧 設定選項

### JWT 設定

```json
{
  "Jwt": {
    "SecretKey": "your-256-bit-secret-key",
    "Issuer": "restaurant-order-api", 
    "Audience": "restaurant-order-client"
  }
}
```

### Google OAuth 設定

```json
{
  "Auth": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

## 📊 資料庫結構

主要資料表：
- `menu_category` - 選單分類
- `menu_item` - 選單項目  
- `menu_price` - 每日價格
- `order` - 訂單
- `order_item` - 訂單項目
- `payment` - 支付記錄
- `audit_log` - 稽核日誌
- Identity 相關表格

## 🧪 測試

執行 API 測試腳本：

```bash
# 給予執行權限
chmod +x test-api.sh

# 執行測試 (需要 jq 工具)
./test-api.sh
```

## 📈 監控與日誌

- 日誌檔案位置: `logs/restaurant-order-*.log`
- 健康檢查端點: `/health`
- Swagger 文檔: `/swagger`

## 🏆 生產部署建議

1. **安全性**
   - 使用強密碼的 JWT SecretKey
   - 啟用 HTTPS
   - 設定適當的 CORS 政策
   - 定期更新相依套件

2. **效能**
   - 啟用資料庫連線池
   - 設定適當的記憶體限制
   - 使用 Redis 進行快取 (可選)

3. **可靠性**
   - 設定資料庫備份
   - 設定健康檢查
   - 使用 Load Balancer

## 🤝 開發團隊

此專案按照餐廳點餐系統的最佳實務開發，適合中小型餐廳使用。

---

**技術支援**: 如有問題請查看 Swagger 文檔或檢查應用程式日誌。

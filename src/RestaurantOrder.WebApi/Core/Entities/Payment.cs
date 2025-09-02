using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrder.WebApi.Core.Entities;

public enum PaymentMethod
{
    Cash,
    CreditCard,
    DebitCard,
    DigitalWallet,
    BankTransfer
}

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    public PaymentMethod Method { get; set; }
    
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    
    public Guid? ReceivedBy { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; } = null!;
}

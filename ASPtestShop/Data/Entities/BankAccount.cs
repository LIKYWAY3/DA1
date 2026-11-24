using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Data.Entities 
{
    public class BankAccount
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required, StringLength(100)]
        public string BankName { get; set; } = null!;

        [Required, StringLength(100)]
        public string AccountName { get; set; } = null!; 

        [Required, StringLength(50)]
        public string AccountNumber { get; set; } = null!;

        public string? Branch { get; set; }

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
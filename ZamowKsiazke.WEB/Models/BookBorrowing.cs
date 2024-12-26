using System.ComponentModel.DataAnnotations;

namespace ZamowKsiazke.Models
{
    public class BookBorrowing
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int BookId { get; set; }
        
        [Required]
        public DateTime BorrowDate { get; set; }
        
        public DateTime? ReturnDate { get; set; }
        
        public bool IsReturned { get; set; }

        [Required]
        public int BorrowingDays { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public int BorrowingPrice { get; set; }

        [Required]
        public string ContactPreference { get; set; } = "Email"; // Email or Phone

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed
        
        // Navigation properties
        public virtual DefaultUser? User { get; set; }
        public virtual Book? Book { get; set; }
    }
}

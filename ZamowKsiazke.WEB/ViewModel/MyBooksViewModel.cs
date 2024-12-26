using ZamowKsiazke.Models;

namespace ZamowKsiazke.ViewModel
{
    public class MyBooksViewModel
    {
        public IEnumerable<BookBorrowing> BorrowedBooks { get; set; } = new List<BookBorrowing>();
        public IEnumerable<Order> PurchasedBooks { get; set; } = new List<Order>();
    }
}

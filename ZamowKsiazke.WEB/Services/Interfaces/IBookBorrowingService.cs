using ZamowKsiazke.Models;

namespace ZamowKsiazke.Services.Interfaces
{
    public interface IBookBorrowingService
    {
        Task<BookBorrowing> BorrowBookAsync(string userId, int bookId, int borrowingDays, string contactPreference);
        Task<BookBorrowing> ReturnBookAsync(int borrowingId);
        Task<IEnumerable<BookBorrowing>> GetUserBorrowingsAsync(string userId);
        Task<IEnumerable<BookBorrowing>> GetBookBorrowingHistoryAsync(int bookId);
        Task<IEnumerable<(Book Book, int BorrowCount)>> GetPopularBooksReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> IsBookAvailableAsync(int bookId);
        Task<Book> GetBookAsync(int bookId);
        Task<BookBorrowing> UpdateBorrowingStatusAsync(int borrowingId, string status);
        Task<IEnumerable<BookBorrowing>> GetAllBorrowingsAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using ZamowKsiazke.Data;
using ZamowKsiazke.Models;
using ZamowKsiazke.Services.Interfaces;

namespace ZamowKsiazke.Services
{
    public class BookBorrowingService : IBookBorrowingService
    {
        private readonly ZamowKsiazkeContext _context;
        private readonly IUserActivityService _userActivityService;

        public BookBorrowingService(ZamowKsiazkeContext context, IUserActivityService userActivityService)
        {
            _context = context;
            _userActivityService = userActivityService;
        }

        public async Task<Book> GetBookAsync(int bookId)
        {
            return await _context.Book.FindAsync(bookId) 
                ?? throw new InvalidOperationException("Book not found");
        }

        public async Task<BookBorrowing> BorrowBookAsync(string userId, int bookId, int borrowingDays, string contactPreference)
        {
            var book = await GetBookAsync(bookId);

            if (!book.IsAvailableForBorrowing)
            {
                throw new InvalidOperationException("Book is not available for borrowing");
            }

            if (borrowingDays <= 0 || borrowingDays > book.MaxBorrowingDays)
            {
                throw new InvalidOperationException($"Invalid borrowing duration. Maximum allowed days: {book.MaxBorrowingDays}");
            }

            var isAvailable = await IsBookAvailableAsync(bookId);
            if (!isAvailable)
            {
                throw new InvalidOperationException("Book is currently borrowed by someone else");
            }

            var borrowing = new BookBorrowing
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow,
                IsReturned = false,
                BorrowingDays = borrowingDays,
                BorrowingPrice = book.BorrowingPrice ?? 0,
                ContactPreference = contactPreference,
                Status = "Pending"
            };

            _context.BookBorrowings.Add(borrowing);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(userId, "BookBorrow", 
                $"Borrowed book: {book.Title} for {borrowingDays} days", borrowing.Id.ToString());

            return borrowing;
        }

        public async Task<BookBorrowing> ReturnBookAsync(int borrowingId)
        {
            var borrowing = await _context.BookBorrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == borrowingId)
                ?? throw new InvalidOperationException("Borrowing record not found");

            if (borrowing.IsReturned)
            {
                throw new InvalidOperationException("Book is already returned");
            }

            borrowing.IsReturned = true;
            borrowing.ReturnDate = DateTime.UtcNow;
            borrowing.Status = "Completed";

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(borrowing.UserId, "BookReturn", 
                $"Returned book: {borrowing.Book?.Title}", borrowing.Id.ToString());

            return borrowing;
        }

        public async Task<IEnumerable<BookBorrowing>> GetUserBorrowingsAsync(string userId)
        {
            return await _context.BookBorrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookBorrowing>> GetBookBorrowingHistoryAsync(int bookId)
        {
            return await _context.BookBorrowings
                .Include(b => b.User)
                .Where(b => b.BookId == bookId)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<(Book Book, int BorrowCount)>> GetPopularBooksReportAsync(
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            var query = _context.BookBorrowings.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(b => b.BorrowDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(b => b.BorrowDate <= endDate.Value);

            var popularBooks = await query
                .GroupBy(b => b.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .ToListAsync();

            var result = new List<(Book Book, int BorrowCount)>();

            foreach (var item in popularBooks)
            {
                var book = await _context.Book.FindAsync(item.BookId);
                if (book != null)
                {
                    result.Add((book, item.BorrowCount));
                }
            }

            return result;
        }

        public async Task<bool> IsBookAvailableAsync(int bookId)
        {
            return !await _context.BookBorrowings
                .AnyAsync(b => b.BookId == bookId && !b.IsReturned);
        }

        public async Task<BookBorrowing> UpdateBorrowingStatusAsync(int borrowingId, string status)
        {
            var borrowing = await _context.BookBorrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == borrowingId)
                ?? throw new InvalidOperationException("Borrowing record not found");

            if (borrowing.Status == "Completed")
            {
                throw new InvalidOperationException("Cannot update status of completed borrowing");
            }

            borrowing.Status = status;
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(borrowing.UserId, "BorrowingStatusUpdate", 
                $"Updated borrowing status to {status} for book: {borrowing.Book?.Title}", borrowing.Id.ToString());

            return borrowing;
        }

        public async Task<IEnumerable<BookBorrowing>> GetAllBorrowingsAsync()
        {
            return await _context.BookBorrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
        }
    }
}

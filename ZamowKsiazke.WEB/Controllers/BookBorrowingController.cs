using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZamowKsiazke.Models;
using ZamowKsiazke.Services.Interfaces;
using ZamowKsiazke.ViewModel;

namespace ZamowKsiazke.Controllers
{
    [Authorize]
    public class BookBorrowingController : Controller
    {
        private readonly IBookBorrowingService _borrowingService;
        private readonly IOrderService _orderService;
        private readonly UserManager<DefaultUser> _userManager;

        public BookBorrowingController(
            IBookBorrowingService borrowingService,
            IOrderService orderService,
            UserManager<DefaultUser> userManager)
        {
            _borrowingService = borrowingService;
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyBorrowings()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var borrowings = await _borrowingService.GetUserBorrowingsAsync(userId);
            var orders = await _orderService.GetUserOrdersAsync(userId);

            var viewModel = new MyBooksViewModel
            {
                BorrowedBooks = borrowings,
                PurchasedBooks = orders
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook(int bookId, int borrowingDays, string contactPreference)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                await _borrowingService.BorrowBookAsync(userId, bookId, borrowingDays, contactPreference);
                TempData["Success"] = "Twoje zamówienie na wypożyczenie książki zostało przyjęte.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Store", new { id = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(int borrowingId, int bookId)
        {
            try
            {
                await _borrowingService.ReturnBookAsync(borrowingId);
                TempData["Success"] = "Książka została zwrócona pomyślnie";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Store", new { id = bookId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRequests()
        {
            var borrowings = await _borrowingService.GetAllBorrowingsAsync();
            return View(borrowings);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateBorrowingStatus(int borrowingId, string status)
        {
            try
            {
                await _borrowingService.UpdateBorrowingStatusAsync(borrowingId, status);
                TempData["Success"] = "Status wypożyczenia został zaktualizowany";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(ManageRequests));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PopularBooks(DateTime? startDate, DateTime? endDate)
        {
            var popularBooks = await _borrowingService.GetPopularBooksReportAsync(startDate, endDate);
            
            var viewModels = popularBooks.Select(pb => new BookBorrowingReportViewModel
            {
                BookTitle = pb.Book.Title,
                Author = pb.Book.Author,
                BorrowCount = pb.BorrowCount
            });

            return View(viewModels);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BookHistory(int id)
        {
            var history = await _borrowingService.GetBookBorrowingHistoryAsync(id);
            return View(history);
        }
    }
}

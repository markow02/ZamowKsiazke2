using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZamowKsiazke.Models;
using ZamowKsiazke.Services.Interfaces;

namespace ZamowKsiazke.ViewComponents
{
    public class BookBorrowingStatusViewComponent : ViewComponent
    {
        private readonly IBookBorrowingService _borrowingService;
        private readonly UserManager<DefaultUser> _userManager;

        public BookBorrowingStatusViewComponent(
            IBookBorrowingService borrowingService,
            UserManager<DefaultUser> userManager)
        {
            _borrowingService = borrowingService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(int bookId)
        {
            var userId = _userManager.GetUserId(UserClaimsPrincipal);
            if (userId == null)
                return Content("Error: User not found");

            var isAvailable = await _borrowingService.IsBookAvailableAsync(bookId);
            var userBorrowings = await _borrowingService.GetUserBorrowingsAsync(userId);
            var currentBorrowing = userBorrowings.FirstOrDefault(b => b.BookId == bookId && !b.IsReturned);
            var book = await _borrowingService.GetBookAsync(bookId);

            var model = new BookBorrowingStatusViewModel
            {
                BookId = bookId,
                IsAvailable = isAvailable,
                CurrentBorrowingId = currentBorrowing?.Id,
                IsAvailableForBorrowing = book.IsAvailableForBorrowing,
                MaxBorrowingDays = book.MaxBorrowingDays,
                BorrowingPrice = book.BorrowingPrice
            };

            return View(model);
        }
    }

    public class BookBorrowingStatusViewModel
    {
        public int BookId { get; set; }
        public bool IsAvailable { get; set; }
        public int? CurrentBorrowingId { get; set; }
        public bool IsAvailableForBorrowing { get; set; }
        public int? MaxBorrowingDays { get; set; }
        public int? BorrowingPrice { get; set; }
        public string? ContactPreference { get; set; }
    }
}

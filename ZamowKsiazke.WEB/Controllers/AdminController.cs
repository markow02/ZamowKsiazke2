﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZamowKsiazke.ViewModel;
using ZamowKsiazke.Models;
using Microsoft.AspNetCore.Authorization;
using ZamowKsiazke.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ZamowKsiazke.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<DefaultUser> _userManager;
    private readonly IUserActivityService _userActivityService;
    private readonly IOrderService _orderService;
    private readonly IBookBorrowingService _bookBorrowingService;

    public AdminController(
        RoleManager<IdentityRole> roleManager, 
        UserManager<DefaultUser> userManager,
        IUserActivityService userActivityService,
        IOrderService orderService,
        IBookBorrowingService bookBorrowingService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _userActivityService = userActivityService;
        _orderService = orderService;
        _bookBorrowingService = bookBorrowingService;
    }


        [HttpGet]
        public async Task<IActionResult> ActivityReport()
        {
            var viewModel = new UserActivityReportViewModel
            {
                RecentActivities = (await _userActivityService.GetRecentActivitiesAsync(10)).ToList(),
                ActivityTypeStats = await _userActivityService.GetActivityStatisticsAsync(),
                UserLoginStats = await _userActivityService.GetUserLoginStatisticsAsync(),
                TotalActivities = (await _userActivityService.GetAllActivitiesAsync()).Count(),
                UniqueUsers = (await _userActivityService.GetAllActivitiesAsync())
                    .Select(a => a.UserId)
                    .Distinct()
                    .Count(),
                ReportGeneratedAt = DateTime.UtcNow
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UserLoginReport()
        {
            var loginStats = await _userActivityService.GetUserLoginStatisticsAsync();
            var users = await _userManager.Users
                .Where(u => u.UserName != null)
                .ToDictionaryAsync(u => u.Id, u => u.UserName!);

            var viewModel = loginStats.Select(stat => new UserActivitySummary
            {
                UserId = stat.Key,
                UserName = users.GetValueOrDefault(stat.Key, "Unknown"),
                LoginCount = stat.Value,
                LastActive = _userActivityService.GetUserActivitiesAsync(stat.Key).Result
                    .OrderByDescending(a => a.Timestamp)
                    .FirstOrDefault()?.Timestamp ?? DateTime.MinValue
            }).OrderByDescending(x => x.LoginCount).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> RecentActivities()
        {
            var activities = await _userActivityService.GetRecentActivitiesAsync(50);
            return View(activities);
        }

        [HttpGet]
        public async Task<IActionResult> ManageOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var viewModels = new List<OrderManagementViewModel>();

            foreach (var order in orders)
            {
                var user = await _userManager.FindByIdAsync(order.UserId);
                if (user?.UserName == null) continue;

                viewModels.Add(new OrderManagementViewModel
                {
                    OrderId = order.Id,
                    UserName = user.UserName,
                    OrderDate = order.OrderDate,
                    PaymentDeadline = order.PaymentDeadline,
                    IsPaid = order.IsPaid,
                    OrderStatus = order.OrderStatus,
                    OrderTotal = order.OrderTotal,
                    OrderItems = order.OrderItems
                });
            }

            return View(viewModels.OrderByDescending(o => o.OrderDate));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(int orderId, bool isPaid)
        {
            try
            {
                // First get the order to ensure it exists
                var order = await _orderService.GetOrderWithItemsAsync(orderId);
                if (order == null)
                {
                    TempData["Error"] = $"Nie znaleziono zamówienia #{orderId}.";
                    return RedirectToAction(nameof(ManageOrders));
                }

                // Update the payment status
                await _orderService.UpdatePaymentStatusAsync(orderId, isPaid);

                // Get the updated order to confirm changes
                var updatedOrder = await _orderService.GetOrderWithItemsAsync(orderId);
                if (updatedOrder.IsPaid != isPaid)
                {
                    throw new Exception("Nie udało się potwierdzić zmiany statusu płatności.");
                }

                // Success message
                TempData["Success"] = isPaid
                    ? $"Potwierdzono płatność za zamówienie #{orderId}. Kwota: {updatedOrder.OrderTotal} zł"
                    : $"Status płatności dla zamówienia #{orderId} został zmieniony na \"Nieopłacono\".";

                // Log the status change
                await _userActivityService.LogActivityAsync(
                    updatedOrder.UserId,
                    "PaymentStatusUpdate",
                    $"Admin zmienił status płatności dla zamówienia #{orderId} na {(isPaid ? "Opłacono" : "Nieopłacono")}.",
                    orderId.ToString()
                );
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = $"Nie znaleziono zamówienia #{orderId}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Wystąpił błąd podczas aktualizacji statusu zamówienia #{orderId}: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageOrders));
        }


        public async Task<IActionResult> BookActivityReport()
        {
            var viewModel = new BookActivityReportViewModel();

            var orders = await _orderService.GetAllOrdersAsync();
            foreach (var order in orders)
            {
                var user = await _userManager.FindByIdAsync(order.UserId);
                if (user?.UserName == null) continue;

                foreach (var item in order.OrderItems)
                {
                    viewModel.PurchaseActivities.Add(new BookPurchaseActivity
                    {
                        UserName = user.UserName,
                        BookTitle = item.Book?.Title ?? "Unknown",
                        Author = item.Book?.Author ?? "Unknown",
                        Price = item.Price,
                        PurchaseDate = order.OrderDate,
                        Quantity = item.Quantity
                    });
                }
            }

            var borrowings = await _bookBorrowingService.GetAllBorrowingsAsync();
            foreach (var borrowing in borrowings)
            {
                var user = await _userManager.FindByIdAsync(borrowing.UserId);
                if (user?.UserName == null) continue;

                viewModel.BorrowingActivities.Add(new BookBorrowingActivity
                {
                    UserName = user.UserName,
                    BookTitle = borrowing.Book?.Title ?? "Unknown",
                    Author = borrowing.Book?.Author ?? "Unknown",
                    BorrowingPrice = borrowing.BorrowingPrice,
                    BorrowingDays = borrowing.BorrowingDays,
                    BorrowDate = borrowing.BorrowDate,
                    ReturnDate = borrowing.ReturnDate,
                    Status = borrowing.Status,
                    IsReturned = borrowing.IsReturned
                });
            }

            viewModel.TotalPurchases = viewModel.PurchaseActivities.Sum(p => p.Quantity);
            viewModel.TotalBorrowings = viewModel.BorrowingActivities.Count;
            viewModel.TotalRevenue = viewModel.PurchaseActivities.Sum(p => p.Price * p.Quantity);
            viewModel.TotalBorrowingRevenue = viewModel.BorrowingActivities.Sum(b => b.BorrowingPrice);

            return View(viewModel);
        }
    }
}

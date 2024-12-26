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

        //[HttpGet]
        //public IActionResult ListAllRoles()
        //{
        //    var roles = _roleManager.Roles;
        //    return View(roles);
        //}

        //[HttpGet]
        //public IActionResult CreateRole()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        IdentityRole identityRole = new IdentityRole
        //        {
        //            Name = model.RoleName
        //        };

        //        var result = await _roleManager.CreateAsync(identityRole);

        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("ListAllRoles");
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> EditRole(string id)
        //{
        //    var role = await _roleManager.FindByIdAsync(id);
        //    if (role == null)
        //    {
        //        ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
        //        return View("Error");
        //    }

        //    if (role.Name == null)
        //    {
        //        ViewBag.ErrorMessage = $"Role name is null for Id = {id}";
        //        return View("Error");
        //    }

        //    EditRoleViewModel model = new EditRoleViewModel
        //    {
        //        Id = role.Id,
        //        RoleName = role.Name
        //    };

        //    foreach (var user in _userManager.Users)
        //    {
        //        if (user.UserName != null && await _userManager.IsInRoleAsync(user, role.Name))
        //        {
        //            model.Users.Add(user.UserName);
        //        }
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditRole(EditRoleViewModel model)
        //{
        //    var role = await _roleManager.FindByIdAsync(model.Id);
        //    if (role == null)
        //    {
        //        ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
        //        return View("Error");
        //    }
        //    else
        //    {
        //        if (model.RoleName == null)
        //        {
        //            ModelState.AddModelError("", "Role name cannot be null");
        //            return View(model);
        //        }

        //        role.Name = model.RoleName;
        //        var result = await _roleManager.UpdateAsync(role);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("ListAllRoles");
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> EditUsersInRole(string id)
        //{
        //    var role = await _roleManager.FindByIdAsync(id);

        //    if (role == null)
        //    {
        //        ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
        //        return View("Error");
        //    }

        //    ViewData["roleId"] = id;
        //    ViewData["roleName"] = role.Name;

        //    var model = new List<UserRoleViewModel>();

        //    if (role.Name == null)
        //    {
        //        ViewData["ErrorMessage"] = $"Role name is null for Id '{id}'";
        //        return View("Error");
        //    }

        //    foreach (var user in _userManager.Users)
        //    {
        //        if (user.UserName != null)
        //        {
        //            UserRoleViewModel userRoleVM = new()
        //            {
        //                Id = user.Id,
        //                Name = user.UserName
        //            };

        //            if (await _userManager.IsInRoleAsync(user, role.Name))
        //            {
        //                userRoleVM.IsSelected = true;
        //            }
        //            else
        //            {
        //                userRoleVM.IsSelected = false;
        //            }

        //            model.Add(userRoleVM);
        //        }
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditUsersInRole(string id, List<UserRoleViewModel> model)
        //{
        //    var role = await _roleManager.FindByIdAsync(id);

        //    if (role == null)
        //    {
        //        ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
        //        return View("Error");
        //    }

        //    for (int i = 0; i < model.Count; i++)
        //    {
        //        var user = await _userManager.FindByIdAsync(model[i].Id);
        //        if (user != null && role.Name != null)
        //        {
        //            IdentityResult? result = null;

        //            if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
        //            {
        //                result = await _userManager.AddToRoleAsync(user, role.Name);
        //            }
        //            else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
        //            {
        //                result = await _userManager.RemoveFromRoleAsync(user, role.Name);
        //            }

        //            if (result != null && !result.Succeeded)
        //            {
        //                ModelState.AddModelError("", "Cannot update user roles");
        //                return View(model);
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Cannot update user roles");
        //            return View(model);
        //        }
        //    }

        //    return RedirectToAction("EditRole", new { Id = id });
        //}
        //[HttpPost]
        //public async Task<IActionResult> DeleteRole(string id)
        //{
        //    var role = await _roleManager.FindByIdAsync(id);
        //    if (role == null)
        //    {
        //        ViewBag.ErrorMessage = $"Rola z Id = {id} nie została znaleziona";
        //        return View("Error");
        //    }
            
        //    var result = await _roleManager.DeleteAsync(role);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ListAllRoles");
        //    }

        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error.Description);
        //    }
        //    return View("ListAllRoles", _roleManager.Roles);
        //}

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

            // Get all orders with items and user information
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

            // Get all borrowings with book and user information
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

            // Calculate totals
            viewModel.TotalPurchases = viewModel.PurchaseActivities.Sum(p => p.Quantity);
            viewModel.TotalBorrowings = viewModel.BorrowingActivities.Count;
            viewModel.TotalRevenue = viewModel.PurchaseActivities.Sum(p => p.Price * p.Quantity);
            viewModel.TotalBorrowingRevenue = viewModel.BorrowingActivities.Sum(b => b.BorrowingPrice);

            return View(viewModel);
        }
    }
}

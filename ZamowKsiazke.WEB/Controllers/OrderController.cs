﻿﻿﻿﻿﻿using Microsoft.AspNetCore.Mvc;
using ZamowKsiazke.Data;
using ZamowKsiazke.Models;
using ZamowKsiazke.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;

namespace ZamowKsiazke.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly Cart _cart;
        private readonly IOrderService _orderService;
        private readonly IUserActivityService _activityService;

        public OrderController(Cart cart, IOrderService orderService, IUserActivityService activityService)
        {
            _cart = cart;
            _orderService = orderService;
            _activityService = activityService;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cartItems = _cart.GetAllCartItems();
            _cart.CartItems = cartItems;

            if (_cart.CartItems.Count == 0)
            {
                ModelState.AddModelError("", "Twój koszyk jest pusty, dodaj książki by kontynuować");
                return View(order);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                ModelState.AddModelError("", "Musisz być zalogowany, aby złożyć zamówienie");
                return View(order);
            }

            order.UserId = userId;
            order.OrderTotal = _cart.GetCartTotal();
            var createdOrder = await _orderService.CreateOrderAsync(userId, _cart);
            _cart.ClearCart();
            return RedirectToAction("CheckoutComplete", new { orderId = createdOrder.Id });
        }

        public async Task<IActionResult> CheckoutComplete(int orderId)
        {
            var order = await _orderService.GetOrderWithItemsAsync(orderId);
            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var orders = await _orderService.GetUserOrdersAsync(userId!);
            return View(orders);
        }

    }
}

﻿﻿﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZamowKsiazke.Data;
using ZamowKsiazke.Models;

namespace ZamowKsiazke.Controllers
{
    public class CartController : Controller
    {
        private readonly ZamowKsiazkeContext _context;
        private readonly Cart _cart;

        public CartController(ZamowKsiazkeContext context, Cart cart)
        {
            _context = context;
            _cart = cart;
        }
        public IActionResult Index()
        {
            var items = _cart.GetAllCartItems();
            _cart.CartItems = items;

            return View(_cart);
        }

        public IActionResult AddToCart(int id)
        {
            var selectedBook = GetBookById(id);

            if (selectedBook != null)
            {
                _cart.AddToCart(selectedBook, quantity: 1);
                TempData["Success"] = "Książka została dodana do koszyka";
                return RedirectToAction("Index", "Cart");
            }

            TempData["Error"] = "Nie można dodać książki do koszyka";
            return RedirectToAction("Index", "Store");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var selectedBook = GetBookById(id);
            if (selectedBook != null)
            {
                _cart.RemoveFromCart(selectedBook);
            }
            return RedirectToAction("Index");
        }

        public IActionResult ReduceQuantity(int id)
        {
            var selectedBook = GetBookById(id);
            if (selectedBook != null)
            {
                _cart.ReduceQuantity(selectedBook);
            }
            return RedirectToAction("Index");
        }

        public IActionResult IncreaseQuantity(int id)
        {
            var selectedBook = GetBookById(id);
            if (selectedBook != null)
            {
                _cart.IncreaseQuantity(selectedBook);
            }
            return RedirectToAction("Index");
        }

        public IActionResult ClearCart()
        {
            _cart.ClearCart();
            return RedirectToAction("Index");
        }

        public Book? GetBookById(int id)
        {
            return _context.Book.FirstOrDefault(b => b.Id == id);
        }
    }
}

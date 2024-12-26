﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using Microsoft.EntityFrameworkCore;
using ZamowKsiazke.Data;
using ZamowKsiazke.Models;
using ZamowKsiazke.Services.Interfaces;
using ZamowKsiazke.Services.Extensions;
using System.Threading.Tasks;

namespace ZamowKsiazke.Services
{
    public class OrderService : IOrderService
    {
        private readonly ZamowKsiazkeContext _context;
        private readonly IUserActivityService _activityService;

        public OrderService(ZamowKsiazkeContext context, IUserActivityService activityService)
        {
            _context = context;
            _activityService = activityService;
        }

        public async Task<Order> GetOrderWithItemsAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            }

            return order;
        }

        public async Task<Order> CreateOrderAsync(string userId, Cart cart)
        {
            if (cart.CartItems == null)
            {
                throw new InvalidOperationException("Cart items cannot be null");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                PaymentDeadline = DateTime.UtcNow.AddHours(48),
                IsPaid = false,
                OrderItems = cart.CartItems.Select(item => new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = item.Book?.Price ?? 0
                }).ToList()
            };

            order.CalculateTotal();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Log the order creation
            await _activityService.LogOrderCreatedAsync(userId, order.Id.ToString());

            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task UpdatePaymentStatusAsync(int orderId, bool isPaid)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            }

            order.CalculateTotal();
            order.IsPaid = isPaid;
            order.OrderStatus = isPaid ? "Opłacono" : "Oczekuje na płatność";
            order.PaymentDeadline = isPaid ? DateTime.UtcNow : DateTime.UtcNow.AddHours(48);

            // Ensure changes are tracked and saved
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            // Verify the changes were saved
            await _context.Entry(order).ReloadAsync();
            if (order.IsPaid != isPaid || order.OrderStatus != (isPaid ? "Opłacono" : "Oczekuje na płatność"))
            {
                throw new Exception("Nie udało się zaktualizować statusu zamówienia.");
            }
            
            // Log the payment status update with more detailed information
            await _activityService.LogActivityAsync(
                order.UserId, 
                "OrderPaymentUpdate", 
                $"Order {orderId} payment status updated to {order.OrderStatus}. Total: {order.OrderTotal} zł"
            );
        }

        public async Task<bool> IsOrderPaidAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            }

            return order.IsPaid;
        }
    }
}

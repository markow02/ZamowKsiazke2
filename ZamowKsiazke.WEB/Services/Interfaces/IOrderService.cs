﻿﻿﻿﻿﻿using ZamowKsiazke.Models;

namespace ZamowKsiazke.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> GetOrderWithItemsAsync(int orderId);
        Task<Order> CreateOrderAsync(string userId, Cart cart);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task UpdatePaymentStatusAsync(int orderId, bool isPaid);
        Task<bool> IsOrderPaidAsync(int orderId);
    }
}

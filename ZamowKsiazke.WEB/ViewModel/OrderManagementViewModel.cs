using System;
using ZamowKsiazke.Models;

namespace ZamowKsiazke.ViewModel
{
    public class OrderManagementViewModel
    {
        public int OrderId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public bool IsPaid { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public int OrderTotal { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

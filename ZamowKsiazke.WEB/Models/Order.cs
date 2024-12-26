﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZamowKsiazke.Models
{
    public class Order
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
            OrderDate = DateTime.UtcNow;
            PaymentDeadline = DateTime.UtcNow.AddHours(48);
            IsPaid = false;
            UpdateOrderStatus();
        }

        private void UpdateOrderStatus()
        {
            OrderStatus = IsPaid ? "Opłacono" : "Oczekuje na płatność";
        }

        public void SetPaymentStatus(bool isPaid)
        {
            IsPaid = isPaid;
            UpdateOrderStatus();
            if (isPaid)
            {
                PaymentDeadline = DateTime.UtcNow;
            }
            else
            {
                PaymentDeadline = DateTime.UtcNow.AddHours(48);
            }
        }

        public int CalculateTotal()
        {
            OrderTotal = OrderItems.Sum(item => item.Price * item.Quantity);
            return OrderTotal;
        }

        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public List<OrderItem> OrderItems { get; set; }
        public int OrderTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public string OrderStatus { get; set; } = "Oczekuje na płatność";

        // Navigation property
        public virtual DefaultUser? User { get; set; }
    }
}

﻿﻿using System.ComponentModel.DataAnnotations;

namespace ZamowKsiazke.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
        public int Quantity { get; set; }
        
        [Required]
        public string CartId { get; set; } = string.Empty;
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace ZamowKsiazke.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany")]
        [MaxLength(1000)]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Język jest wymagany")]
        public required string Language { get; set; }

        [Required(ErrorMessage = "ISBN jest wymagany")]
        [MaxLength(13)]
        public required string ISBN { get; set; }

        [Required(ErrorMessage = "Data publikacji jest wymagana")]
        [DataType(DataType.Date)]
        [Display(Name = "Data publikacji")]
        public DateTime DatePublished { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [DataType(DataType.Currency)]
        [Display(Name = "Cena")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Autor jest wymagany")]
        [Display(Name = "Autor")]
        public required string Author { get; set; }

        [Display(Name = "URL obrazu")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Dostępna do wypożyczenia")]
        public bool IsAvailableForBorrowing { get; set; }

        [Display(Name = "Maksymalny okres wypożyczenia (dni)")]
        public int? MaxBorrowingDays { get; set; }

        [Display(Name = "Cena wypożyczenia")]
        [DataType(DataType.Currency)]
        public int? BorrowingPrice { get; set; }

        [Display(Name = "Ilość na stanie")]
        [Required(ErrorMessage = "Ilość na stanie jest wymagana")]
        public int StockQuantity { get; set; }
    }
}

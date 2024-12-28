namespace ZamowKsiazke.ViewModel
{
    public class BookActivityReportViewModel
    {
        public BookActivityReportViewModel()
        {
            PurchaseActivities = new List<BookPurchaseActivity>();
            BorrowingActivities = new List<BookBorrowingActivity>();
            ReportGeneratedAt = DateTime.UtcNow;
        }

        public List<BookPurchaseActivity> PurchaseActivities { get; set; }
        public List<BookBorrowingActivity> BorrowingActivities { get; set; }
        public DateTime ReportGeneratedAt { get; set; }
        public int TotalPurchases { get; set; }
        public int TotalBorrowings { get; set; }
        public int TotalRevenue { get; set; }
        public int TotalBorrowingRevenue { get; set; }
    }

    public class BookPurchaseActivity
    {
        public string UserName { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
    }

    public class BookBorrowingActivity
    {
        public string UserName { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int BorrowingPrice { get; set; }
        public int BorrowingDays { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsReturned { get; set; }
    }
}

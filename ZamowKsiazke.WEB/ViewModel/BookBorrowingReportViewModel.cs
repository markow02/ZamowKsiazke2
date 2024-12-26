namespace ZamowKsiazke.ViewModel
{
    public class BookBorrowingReportViewModel
    {
        public string BookTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int BorrowCount { get; set; }
        public DateTime? LastBorrowed { get; set; }
        public bool IsCurrentlyBorrowed { get; set; }
        public string? CurrentBorrowerName { get; set; }
    }
}

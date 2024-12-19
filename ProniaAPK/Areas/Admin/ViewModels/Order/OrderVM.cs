namespace ProniaAPK.Areas.Admin.ViewModels
{
    public class OrderVM
    {
        public string User { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool? Status { get; set; }
    }
}

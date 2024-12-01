using ProniaAPK.Models;

namespace ProniaAPK.Areas.Admin.ViewModels
{
    public class CreateAdminProductVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public List<Category> Categories { get; set; }

    }
}

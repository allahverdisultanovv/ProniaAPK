using System.ComponentModel.DataAnnotations;

namespace ProniaAPK.Models
{
    public class Category : BaseEntity
    {
        [MaxLength(30, ErrorMessage = "Please be Slow")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}

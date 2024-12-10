namespace ProniaAPK.Models
{
    public class Size : BaseEntity
    {
        public string Name { get; set; }
        //relational
        public List<ProductSize> ProductSizes { get; set; }
    }
}

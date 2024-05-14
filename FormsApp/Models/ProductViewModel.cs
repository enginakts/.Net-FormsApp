namespace FormsApp.Models
{
    public class ProductViewModel
    {
        public List<Product> Products {get; set;} = null!;
        public List<Category> Categories {get; set;} = null!;
        public string? SelectCategory { get; set; } 

    }
}
using System.Data;

namespace FormsApp.Models
{
    public class Repository
    {
        private static readonly List<Product> _products = new();
        private static readonly List<Category> _categorys = new();
        static Repository()
        {
            _categorys.Add(new Category {CategoryId = 1, Name = "Telefon"});
            _categorys.Add(new Category {CategoryId = 2, Name = "Bilgisayar"});

            //* Phone
            _products.Add(new Product {ProductId =1,Name = "Infinix Not 30", Price = 9349,Image = "1.jpg",CategoryId = 1,IsActive = true});
            _products.Add(new Product {ProductId =2,Name = "Samsung Galaxi A34", Price = 12249,Image = "2.jpg", CategoryId = 1 , IsActive = true});
            _products.Add(new Product {ProductId =3,Name = "IPhone 14 pro Max", Price = 80000,Image = "3.jpg", CategoryId = 1 , IsActive = false});
            _products.Add(new Product {ProductId =4,Name = "Infinix Hot 30", Price = 5856,Image = "6.jpg", CategoryId = 1 , IsActive = false});
            _products.Add(new Product {ProductId =5,Name = "Omix X3", Price = 3940,Image = "7.jpg", CategoryId = 1 , IsActive = true});
           
            //* Computer
             _products.Add(new Product {ProductId =6,Name = "Casper Nirvana", Price = 7699,Image = "8.jpg", CategoryId = 2 , IsActive = false});
            _products.Add(new Product {ProductId =7,Name = "Hp Victus", Price = 3899,Image = "4.jpg", CategoryId = 2 , IsActive = true});
            _products.Add(new Product {ProductId =8,Name = "Monster", Price = 26799,Image = "5.jpg", CategoryId = 2 , IsActive = true});
        }
        public static List<Product> Products
        {
            get
            {
                return _products;
            }
        }
        public static void CreateProduct(Product entity)
        {
            _products.Add(entity);

        }
        public static void EditProduct(Product uodateProduct)
        {
            var entity = _products.FirstOrDefault(p =>p.ProductId ==uodateProduct.ProductId );
            if(entity != null)
            {
                if(string.IsNullOrEmpty(uodateProduct.Name)) //? name alanının boş olup olmadıgını kontrol eder
                {
                     entity.Name = uodateProduct.Name;
                }
                entity.Price = uodateProduct.Price;
                entity.Image = uodateProduct.Image;
                entity.CategoryId = uodateProduct.CategoryId;
                entity.IsActive = uodateProduct.IsActive;
            }
        }
        public static void EditIsActive(Product uodateProduct)
        {
            var entity = _products.FirstOrDefault(p =>p.ProductId ==uodateProduct.ProductId );
            if(entity != null)
            {
           
                entity.IsActive = uodateProduct.IsActive;
            }
        }
        public static void DeleteProduct(Product deleteProduct)
        {
            var entity = _products.FirstOrDefault(p => p.ProductId == deleteProduct.ProductId);
            if(entity != null)
            {
                _products.Remove(entity);
            }
        }
        public static List<Category> Categries
        {
            get
            {
                return _categorys;
            }
        }
    }
}
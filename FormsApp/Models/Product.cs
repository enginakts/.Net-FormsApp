using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FormsApp.Models
{
    //todo [Bind("Name","Price")] -> set edilecek kısımları belirler
    public class Product
    {
        [Display (Name = "Urun Id")]
       // [BindNever] //* set edilmesini engeler
        public int ProductId { get; set; }

         [Display (Name = "Urun Adı")]
        [Required (ErrorMessage ="Ürün adı boş bırakılamaz.")] //* Zorunlu alan olmasını saglar
        [StringLength(100)]                                    //* Max girilecek karakter sayısını beliler
        public string Name { get; set; } = null!; //! null bir deger girmeyeceginden emin olundugunda ! kulanılır home/ındex de de var / soz vermek gibi dusunule bilir

         [Display (Name = "Fiyat")]  
         [Required(ErrorMessage ="Fiyat alanı boş bırakılamaz.")] 
         [Range(0,100000)]//* Min ve Max deger belirler
        public decimal? Price { get; set; }

         [Display (Name = "Urun Resmi")] //[Required (ErrorMessage ="Resim alanı boş bırakılamaz.")]
        public string Image { get; set; } = string.Empty;

      
        public bool IsActive { get; set; }

         [Display (Name = "Kategori")] [Required (ErrorMessage ="Kategori alanı boş bırakılamaz.")]
        public int? CategoryId { get; set; }
    }
}
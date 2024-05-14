using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormsApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace FormsApp.Controllers;

public class HomeController : Controller
{
   

    public HomeController()
    {
       
    }

    [HttpGet]
    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;

        if(!String.IsNullOrEmpty(searchString))
        {
            ViewBag.searchString = searchString;
            products = products.Where(p=> p.Name.ToLower().Contains(searchString.ToLower())).ToList(); //! p.Name! / null bir deger girmeyeceginden emin olundugunda ! kulanılır
        }
        if(!String.IsNullOrEmpty(category) && category != "0")
        {
            products = products.Where(p => p.CategoryId == int.Parse(category)).ToList();
        }

       //* ViewBag.Categries = new SelectList(Repository.Categries, "CategoryId","Name",category);

        var model = new ProductViewModel
        {
            Products = products,
            Categories = Repository.Categries,
            SelectCategory = category
        };
        return View(model);
    }

    [HttpGet]
     public IActionResult Create()
    {
      //*  ViewBag.Categories = Repository.Categries;
        ViewBag.Categories =new SelectList(Repository.Categries, "CategoryId","Name");
        return View();
    }
    [HttpPost]
     public async Task<IActionResult> Create(Product model , IFormFile imageFile)
    {
        
        var extension = "";
       
        if(imageFile !=  null) //* imgFail null olma durumu
        {
            var alloweExtensions = new[] {".jpg",".png","jpeg"}; //* kubul edilebilir uzantı dizisi
            extension = Path.GetExtension(imageFile.FileName); //* .jpg benzeri kısmı alır 
            if(!alloweExtensions.Contains(extension)) //* uzantı kontrolü
            {
                ModelState.AddModelError("","Gecerli bir resim seciniz.");
            }
        }

        
        if(ModelState.IsValid) //* istenilen bilgileri kulanıcı girdi urun kayıtislemi yapila bilir
        {
            if(imageFile != null)
            {
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}"); // resme randım bir isim olusturur //! eklenen resim ile aynı isimde baska bir resim olma ihtimali vardı 
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img",randomFileName); //* Kulanıcının ekledigi resim dosyasını alıp wwwroot altında ki img klasorune ekler

                using(var strem = new FileStream(path,FileMode.Create)) //* resmi kaydetme islemi
                {
                    await imageFile.CopyToAsync(strem);
                }
            model.Image = randomFileName; //* ımage bilgisini gunceler
            model.ProductId = Repository.Products.Count() + 1;
            Repository.CreateProduct(model);
            return RedirectToAction("Index"); //todo formun tekrar kulanıcının karsısına gelmemesi icin 
            }
            
        }
        ViewBag.Categories =new SelectList(Repository.Categries, "CategoryId","Name");
       return View(model);
     
    }

    public IActionResult Privacy()
    {
        
        return View();
    }

  public IActionResult Edit(int? id)
  {
    if(id == null)
    {
        return NotFound();
    }
    var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
    if(entity == null)
    {
        return NotFound();
    }
    ViewBag.Categories =new SelectList(Repository.Categries, "CategoryId","Name");
    return View(entity);
  }

  [HttpPost]
  public async Task<IActionResult> Edit(int id ,Product model, IFormFile? imageFile)
  {

    if(id != model.ProductId)
    {
        return NotFound();
    }
    if(ModelState.IsValid)
    {
        
        if(imageFile != null)
        {
            var extension = Path.GetExtension(imageFile.FileName);
            var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img",randomFileName);
            using(var strem = new FileStream(path,FileMode.Create))
            {
                await imageFile.CopyToAsync(strem);
            }
            model.Image = randomFileName;
        }
        Repository.EditProduct(model);
       return RedirectToAction("Index");
    }
     ViewBag.Categories =new SelectList(Repository.Categries, "CategoryId","Name");
    return View(model);
  }

  public IActionResult Delete(int? id)
  {
    if(id == null)
    {

    }
    var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
    if(entity == null)
    {
        return NotFound();
    }
    return View("DeleteConfirm",entity);

  }
  [HttpPost]
   public IActionResult Delete(int? id,int productId)
   {
    if(id != productId)
    {
        return NotFound();
    }
     var entity = Repository.Products.FirstOrDefault(p => p.ProductId == productId);
    if(entity == null)
    {
        return NotFound();
    }
    Repository.DeleteProduct(entity);
    return RedirectToAction("Index");
   }

[HttpPost]
   public IActionResult EditProducts(List<Product> products)
   {
        foreach(var product in products)
        {
            Repository.EditIsActive(product);
        }
        return RedirectToAction("Index");
   }
}
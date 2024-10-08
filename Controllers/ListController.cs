using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ListController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public IActionResult Index()
        {
            ViewBag.Name = Request.Cookies["Name"];
            var item = db.itemLists.ToList();
            return View(item);
        }
        public IActionResult Create(int Id)
        {
            var user = db.users.Find(Id);
            return View(user);
        }
        [HttpPost]
        public IActionResult Create(ItemList itemList ,IFormFile PhotoUrl)
        {

            var fileName = Upload(PhotoUrl);
            if (fileName != null)
            {
                itemList.Img = fileName;
            }
            else
            {
                itemList.Img = " ";
            }

            db.itemLists.Add(itemList);
            db.SaveChanges();
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Secure=true;
            cookieOptions.Expires = DateTime.Now.AddSeconds(10);
            Response.Cookies.Append("successCookies", "Add Item successfully", cookieOptions);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int Id)
        {
            var Item = db.itemLists.Find(Id);
            return View(Item);
        }

        [HttpPost]
        public IActionResult Edit(ItemList itemList, IFormFile PhotoUrl)
        {
            var old = db.itemLists.Where(e => e.Id == itemList.Id).AsNoTracking().FirstOrDefault();

                if (PhotoUrl != null && PhotoUrl.Length > 0)
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", old.Img);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    var fileName = Upload(PhotoUrl);
                    if (fileName != null)
                    {
                        itemList.Img = fileName;
                    }
                }
                 else
                 {
                    itemList.Img = old.Img;
                 }
                db.itemLists.Update(itemList);
                db.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Delete(int Id)
        {
            var item = db.itemLists.Find(Id);
            if (item != null)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", item.Img);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                db.itemLists.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }



        private string? Upload(IFormFile PhotoUrl)
        {
            if (PhotoUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    PhotoUrl.CopyTo(stream);
                }
                return fileName;
            }
            return null;
        }



    }
}

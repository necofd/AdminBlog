using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdminBlog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminBlog.Controllers;

public class BlogController : Controller
{
    private readonly ILogger<BlogController> _logger;
    private readonly BlogContext _context;//veri tabani context sinifi tanitma

    public BlogController(ILogger<BlogController> logger, BlogContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Index()
    {
        //Category yollar
        ViewBag.Categories = _context.Category.Select(w =>
            new SelectListItem{
                Text = w.Name,
                Value = w.Id.ToString()
            }
        ).ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Save(Blog model){
        if(model != null){
            var file = Request.Form.Files.First(); //formdaki dosyamiza aldik
            //C:\Users\HP\Desktop\MeBlog\wwwroot\img
            string savePath = Path.Combine("C:","Users","HP","Desktop","MeBlog","wwwroot","img");//kaydet
            var fileName  = $"{DateTime.Now:MMddHHmmss}.{file.FileName.Split(".").Last()}";
            var fileUrl = Path.Combine(savePath, fileName);
            //kopyalama islemi
            using (var fileStream = new FileStream(fileUrl,FileMode.Create)){
                await file.CopyToAsync(fileStream);
            }
            model.ImagePath = fileName;
            model.AuthorId = (int)HttpContext.Session.GetInt32("id");
            //veritabanina ekleme
            await _context.AddAsync(model);
            //veritabanina degisikligi uygula
            await _context.SaveChangesAsync();
            return Json(true);
        }

        return Json(false);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
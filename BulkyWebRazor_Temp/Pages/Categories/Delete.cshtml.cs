using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        public readonly ApplicationDbContext _db;
        public Category? Category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            Category = _db.Categories.Find(id);
        }
        public ActionResult OnPost()
        {
            _db.Categories.Remove(Category);
            _db.SaveChanges();
            TempData["Success"] = "Category deleted successfully";
            return RedirectToPage("Index");
        }
    }
}

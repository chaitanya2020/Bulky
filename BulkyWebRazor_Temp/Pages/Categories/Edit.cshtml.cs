using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        public readonly ApplicationDbContext _db;
        public Category? Category { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            Category = _db.Categories.Find(id);
        }

        public IActionResult OnPost()
        {
            _db.Categories.Update(Category);
            _db.SaveChanges();
            TempData["Success"] = "Category updated successfully";
            return RedirectToPage("Index");
        }

    }
}

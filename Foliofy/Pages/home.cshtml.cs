using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Foliofy.DataBase;
using Foliofy.Models;

namespace Foliofy.Pages
{
    public class homeModel : PageModel
    {
        private readonly Database db;
        public homeModel(Database db) { this.db = db; }

        [BindProperty]
        public User User { get; set; }
    }
}

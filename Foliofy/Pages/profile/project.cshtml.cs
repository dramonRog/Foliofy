using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foliofy.Pages.profile
{
    [Authorize]
    public class ProjectModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

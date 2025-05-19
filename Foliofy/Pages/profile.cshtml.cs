using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foliofy.Pages
{
    [Authorize]
    public class profileModel : PageModel
    {
    }
}

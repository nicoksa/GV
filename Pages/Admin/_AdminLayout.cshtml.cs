using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GV.Pages.Admin
{
    [Authorize]
    public class _AdminLayoutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

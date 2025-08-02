using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace GV.Pages.Admin
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

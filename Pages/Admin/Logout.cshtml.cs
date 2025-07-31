using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace GV.Pages.Admin
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // Cerrar autenticaci�n
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Limpiar la sesi�n solo si est� disponible
            if (HttpContext.Session != null)
            {
                HttpContext.Session.Clear();
            }

            // Redirigir a login con mensaje de �xito
            TempData["LogoutSuccess"] = "Has cerrado sesi�n correctamente";
            return RedirectToPage("/Admin/Login");
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Admin/Login");
        }
    }
}
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
            // Cerrar autenticación
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Limpiar la sesión solo si está disponible
            if (HttpContext.Session != null)
            {
                HttpContext.Session.Clear();
            }

            // Redirigir a login con mensaje de éxito
            TempData["LogoutSuccess"] = "Has cerrado sesión correctamente";
            return RedirectToPage("/Admin/Login");
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Admin/Login");
        }
    }
}
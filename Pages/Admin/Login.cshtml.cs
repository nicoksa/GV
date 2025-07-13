using GV.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BCrypt.Net;
using GV.Models;

namespace GV.Pages.Admin
{
    public class LoginModel : PageModel
    {

        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El email es obligatorio")]
            [EmailAddress(ErrorMessage = "Debe ser un email válido")]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Limpieza exhaustiva de inputs
            Input.Email = Input.Email.Trim();
            Input.Password = Input.Password.Trim();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == Input.Email);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado");
                return Page();
            }

            // Verificación con manejo de errores
            bool isValid;
           
            
            try
            {
                isValid = BCrypt.Net.BCrypt.Verify(Input.Password, usuario.PasswordHash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en BCrypt: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error en verificación de contraseña");
                return Page();
            }

            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "Contraseña incorrecta");
                return Page();
            }
            


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim("IsAdmin", usuario.EsAdmin.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

  

            return RedirectToPage("/Admin/Dashboard");
        }
    }
}
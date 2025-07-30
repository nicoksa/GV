using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BCrypt.Net;

namespace GV.Pages.Admin
{
    public class ConfiguracionModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ConfiguracionModel> _logger;

        public ConfiguracionModel(AppDbContext context, ILogger<ConfiguracionModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public UsuarioInfoModel UsuarioInfo { get; set; }

        [BindProperty]
        public CambioPasswordModel CambioPassword { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return RedirectToPage("/Admin/Login");
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            UsuarioInfo = new UsuarioInfoModel
            {
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email
            };

            return Page();
        }

        public async Task<IActionResult> OnPostActualizarInfoAsync()
        {


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return RedirectToPage("/Admin/Login");
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.NombreUsuario = UsuarioInfo.NombreUsuario;
            usuario.Email = UsuarioInfo.Email;
            usuario.FechaActualizacion = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Informaci�n actualizada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar informaci�n de usuario");
                TempData["ErrorMessage"] = "Ocurri� un error al actualizar la informaci�n.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCambiarPasswordAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return RedirectToPage("/Admin/Login");
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Verificar contrase�a actual con BCrypt
            try
            {
                if (!BCrypt.Net.BCrypt.Verify(CambioPassword.PasswordActual, usuario.PasswordHash))
                {
                    ModelState.AddModelError("CambioPassword.PasswordActual", "La contrase�a actual es incorrecta.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar contrase�a");
                ModelState.AddModelError("CambioPassword.PasswordActual", "Error al verificar la contrase�a.");
                return Page();
            }

            // Validar que la nueva contrase�a sea diferente
            try
            {
                if (BCrypt.Net.BCrypt.Verify(CambioPassword.NuevaPassword, usuario.PasswordHash))
                {
                    ModelState.AddModelError("CambioPassword.NuevaPassword", "La nueva contrase�a debe ser diferente a la actual.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comparar contrase�as");
                ModelState.AddModelError("CambioPassword.NuevaPassword", "Error al validar la nueva contrase�a.");
                return Page();
            }

            // Actualizar contrase�a con BCrypt
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(CambioPassword.NuevaPassword);
            usuario.FechaActualizacion = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contrase�a cambiada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contrase�a");
                TempData["ErrorMessage"] = "Ocurri� un error al cambiar la contrase�a.";
            }

            return RedirectToPage();
        }
    }

    public class UsuarioInfoModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string? NombreUsuario { get; set; }

        [Required(ErrorMessage = "El correo electr�nico es requerido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres")]
        [EmailAddress(ErrorMessage = "Formato de correo electr�nico inv�lido")]
        public string? Email { get; set; }
    }

    public class CambioPasswordModel
    {
        [Required(ErrorMessage = "La contrase�a actual es requerida")]
        public string? PasswordActual { get; set; }

        [Required(ErrorMessage = "La nueva contrase�a es requerida")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contrase�a debe tener al menos 8 caracteres")]
        public string? NuevaPassword { get; set; }

        [Required(ErrorMessage = "Debe confirmar la nueva contrase�a")]
        [Compare("NuevaPassword", ErrorMessage = "Las contrase�as no coinciden")]
        public string? ConfirmarPassword { get; set; }
    }
}
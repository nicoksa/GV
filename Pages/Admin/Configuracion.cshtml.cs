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
                TempData["SuccessMessage"] = "Información actualizada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar información de usuario");
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar la información.";
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

            // Verificar contraseña actual con BCrypt
            try
            {
                if (!BCrypt.Net.BCrypt.Verify(CambioPassword.PasswordActual, usuario.PasswordHash))
                {
                    ModelState.AddModelError("CambioPassword.PasswordActual", "La contraseña actual es incorrecta.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar contraseña");
                ModelState.AddModelError("CambioPassword.PasswordActual", "Error al verificar la contraseña.");
                return Page();
            }

            // Validar que la nueva contraseña sea diferente
            try
            {
                if (BCrypt.Net.BCrypt.Verify(CambioPassword.NuevaPassword, usuario.PasswordHash))
                {
                    ModelState.AddModelError("CambioPassword.NuevaPassword", "La nueva contraseña debe ser diferente a la actual.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comparar contraseñas");
                ModelState.AddModelError("CambioPassword.NuevaPassword", "Error al validar la nueva contraseña.");
                return Page();
            }

            // Actualizar contraseña con BCrypt
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(CambioPassword.NuevaPassword);
            usuario.FechaActualizacion = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contraseña cambiada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                TempData["ErrorMessage"] = "Ocurrió un error al cambiar la contraseña.";
            }

            return RedirectToPage();
        }
    }

    public class UsuarioInfoModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string? NombreUsuario { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        public string? Email { get; set; }
    }

    public class CambioPasswordModel
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        public string? PasswordActual { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string? NuevaPassword { get; set; }

        [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmarPassword { get; set; }
    }
}
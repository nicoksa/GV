using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace GV.Pages.Admin
{
    [Authorize]
    public class GestionCamposModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public GestionCamposModel(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public List<PropiedadCampo> PropiedadesCampo { get; set; }

        public async Task OnGetAsync()
        {
            PropiedadesCampo = await _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToListAsync();
        }



        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var campo = await _context.PropiedadesCampo
                    .Include(p => p.Imagenes) // Incluir imágenes
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (campo == null)
                {
                    TempData["ErrorMessage"] = "No se encontró el campo a eliminar";
                    return RedirectToPage();
                }

                // Eliminar archivos físicos de imágenes
                foreach (var imagen in campo.Imagenes)
                {
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, imagen.Url.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.PropiedadesCampo.Remove(campo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Campo eliminado correctamente";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el campo: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}

using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;


namespace GV.Pages.Admin
{
    [Authorize]
    public class GestionPropiedadesModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public GestionPropiedadesModel(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public List<PropiedadUrbana> PropiedadesUrbanas { get; set; }

        public async Task OnGetAsync()
        {
            PropiedadesUrbanas = await _context.PropiedadesUrbanas
                .Include(p => p.Imagenes)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var propiedad = await _context.PropiedadesUrbanas
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propiedad != null)
            {
                // Eliminar archivos físicos de imágenes
                foreach (var imagen in propiedad.Imagenes)
                {
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, imagen.Url.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.PropiedadesUrbanas.Remove(propiedad);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Propiedad eliminada correctamente";
            }
            else
            {
                TempData["ErrorMessage"] = "No se encontró la propiedad a eliminar";
            }

            return RedirectToPage();
        }
    }
}

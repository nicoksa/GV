using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GV.Pages
{
    public class PropertyModel : PageModel
    {
        private readonly AppDbContext _context;

        public Propiedad? Propiedad { get; set; }
        public PropiedadUrbana? PropiedadUrbana { get; set; }
        public PropiedadCampo? PropiedadCampo { get; set; }
        public bool IsUrban { get; set; }

        public PropertyModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Propiedad = await _context.Propiedades
                .Include(p => p.Imagenes)
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Propiedad == null)
            {
                return NotFound();
            }

            if (Propiedad.Division == "Urbano")
            {
                PropiedadUrbana = await _context.PropiedadesUrbanas
                    .FirstOrDefaultAsync(pu => pu.Id == id);
                IsUrban = true;
            }
            else if (Propiedad.Division == "Campo")
            {
                PropiedadCampo = await _context.PropiedadesCampo
                    .FirstOrDefaultAsync(pc => pc.Id == id);
                IsUrban = false;
            }

            return Page();
        }
    }
}
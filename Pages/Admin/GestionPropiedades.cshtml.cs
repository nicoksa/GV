using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using GV.Models;
using Microsoft.EntityFrameworkCore;

namespace GV.Pages.Admin
{
    public class GestionPropiedadesModel : PageModel
    {
        private readonly AppDbContext _context;

        public GestionPropiedadesModel(AppDbContext context)
        {
            _context = context;
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
            var propiedad = await _context.Propiedades.FindAsync(id);

            if (propiedad != null)
            {
                _context.Propiedades.Remove(propiedad);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}

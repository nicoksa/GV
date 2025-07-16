using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using GV.Models;
using Microsoft.EntityFrameworkCore;

namespace GV.Pages.Admin
{
    public class GestionCamposModel : PageModel
    {
        private readonly AppDbContext _context;

        public GestionCamposModel(AppDbContext context)
        {
            _context = context;
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
            var propiedad = await _context.PropiedadesCampo.FindAsync(id);

            if (propiedad != null)
            {
                _context.PropiedadesCampo.Remove(propiedad);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}

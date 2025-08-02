using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using GV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GV.Pages.Admin
{
    [Authorize]
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
            try
            {
                var campo = await _context.PropiedadesCampo.FindAsync(id);
                if (campo == null)
                {
                    TempData["ErrorMessage"] = "No se encontró el campo a eliminar";
                    return RedirectToPage();
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

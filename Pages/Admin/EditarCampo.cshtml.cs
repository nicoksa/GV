using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using GV.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GV.Pages.Admin
{
    public class EditarCampoModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditarCampoModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PropiedadCampo Propiedad { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Propiedad = await _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .Include(p => p.Videos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Propiedad == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Agregar logging de errores como en EditarPropiedad
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return Page();
            }

            var propiedadExistente = await _context.PropiedadesCampo
                .FirstOrDefaultAsync(p => p.Id == Propiedad.Id);

            if (propiedadExistente == null)
            {
                return NotFound();
            }

            // Actualizar propiedades básicas (igual que en EditarPropiedad)
            propiedadExistente.Titulo = Propiedad.Titulo;
            propiedadExistente.Descripcion = Propiedad.Descripcion;
            propiedadExistente.Precio = Propiedad.Precio;
            propiedadExistente.Ubicacion = Propiedad.Ubicacion;
            propiedadExistente.EsDestacada = Propiedad.EsDestacada;

            // Asegurar que la división sea "Campo" (IMPORTANTE)
            propiedadExistente.Division = "Campo";

            // Actualizar características específicas de campo
            propiedadExistente.Aptitud = Propiedad.Aptitud;
            propiedadExistente.Hectareas = Propiedad.Hectareas;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/Admin/GestionCampos");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropiedadExists(Propiedad.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool PropiedadExists(int id)
        {
            return _context.PropiedadesCampo.Any(e => e.Id == id);
        }
    }
}
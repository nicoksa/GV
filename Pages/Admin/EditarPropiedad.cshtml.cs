using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using GV.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GV.Pages.Admin
{
    public class EditarPropiedadModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditarPropiedadModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PropiedadUrbana Propiedad { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Propiedad = await _context.PropiedadesUrbanas
                .Include(p => p.Imagenes)
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
                return Page();
            }

            var propiedadExistente = await _context.PropiedadesUrbanas
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == Propiedad.Id);

            if (propiedadExistente == null)
            {
                return NotFound();
            }

            // Actualizar propiedades básicas
            propiedadExistente.Titulo = Propiedad.Titulo;
            propiedadExistente.Descripcion = Propiedad.Descripcion;
            propiedadExistente.Tipo = Propiedad.Tipo;
            propiedadExistente.Precio = Propiedad.Precio;
            propiedadExistente.Ubicacion = Propiedad.Ubicacion;
            propiedadExistente.EsDestacada = Propiedad.EsDestacada;

            // Actualizar características
            propiedadExistente.Ambientes = Propiedad.Ambientes;
            propiedadExistente.Dormitorios = Propiedad.Dormitorios;
            propiedadExistente.Banios = Propiedad.Banios;
            propiedadExistente.SuperficieTotal = Propiedad.SuperficieTotal;
            propiedadExistente.SuperficieCubierta = Propiedad.SuperficieCubierta;
            propiedadExistente.Direccion = Propiedad.Direccion;

            // Actualizar comodidades
            propiedadExistente.Cochera = Propiedad.Cochera;
            propiedadExistente.Patio = Propiedad.Patio;
            propiedadExistente.Aire_acond = Propiedad.Aire_acond;
            propiedadExistente.Seguridad = Propiedad.Seguridad;
            propiedadExistente.Pileta = Propiedad.Pileta;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/Admin/GestionPropiedades");
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
            return _context.Propiedades.Any(e => e.Id == id);
        }
    }
}

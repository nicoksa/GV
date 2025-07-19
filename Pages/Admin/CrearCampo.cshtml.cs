using GV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using System.ComponentModel.DataAnnotations;

namespace GV.Pages.Admin
{
    public class CrearCampoModel : PageModel
    {
        private readonly AppDbContext _context;

        public CrearCampoModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PropiedadCampoInputModel Propiedad { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var propiedad = new PropiedadCampo
            {
                Titulo = Propiedad.Titulo,
                Descripcion = Propiedad.Descripcion,
                Division = "Campo",
                Ubicacion = Propiedad.Ubicacion,
                Precio = Propiedad.Precio,
                EsDestacada = Propiedad.EsDestacada,
                Aptitud = Propiedad.Aptitud,
                Hectareas = Propiedad.Hectareas,
                Imagenes = new List<Imagen>(),
                Videos = new List<Video>()
            };

            _context.PropiedadesCampo.Add(propiedad);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/GestionCampos");
        }

        public class PropiedadCampoInputModel
        {
            [Required(ErrorMessage = "El título es obligatorio")]
            public string Titulo { get; set; }

            public string Descripcion { get; set; }

            [Required(ErrorMessage = "La ubicación es obligatoria")]
            public string Ubicacion { get; set; }

            [Required(ErrorMessage = "El precio es obligatorio")]
            [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
            public decimal Precio { get; set; }

            public bool EsDestacada { get; set; }

            [Required(ErrorMessage = "La aptitud es obligatoria")]
            public string Aptitud { get; set; }

            [Required(ErrorMessage = "Las hectáreas son obligatorias")]
            [Range(1, int.MaxValue, ErrorMessage = "Las hectáreas deben ser mayor a 0")]
            public int Hectareas { get; set; }
        }
    }
}
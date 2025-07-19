using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GV.Pages.Admin
{
    public class CrearPropiedadModel : PageModel
    {
        private readonly AppDbContext _context;

        public CrearPropiedadModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PropiedadUrbanaInputModel Propiedad { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var propiedad = new PropiedadUrbana
            {
                Titulo = Propiedad.Titulo,
                Descripcion = Propiedad.Descripcion,
                Division = "Urbano",
                Ubicacion = Propiedad.Ubicacion,
                Precio = Propiedad.Precio,
                EsDestacada = Propiedad.EsDestacada,
                Tipo = Propiedad.Tipo,
                Ambientes = Propiedad.Ambientes,
                Dormitorios = Propiedad.Dormitorios,
                Banios = Propiedad.Banios,
                SuperficieTotal = Propiedad.SuperficieTotal,
                SuperficieCubierta = Propiedad.SuperficieCubierta,
                Cochera = Propiedad.Cochera,
                Patio = Propiedad.Patio,
                Aire_acond = Propiedad.AireAcondicionado,
                Seguridad = Propiedad.Seguridad,
                Pileta = Propiedad.Pileta,
                Direccion = Propiedad.Direccion,
                Imagenes = new List<Imagen>(),
                Videos = new List<Video>()
            };

            _context.PropiedadesUrbanas.Add(propiedad);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/GestionPropiedades");
        }

        public class PropiedadUrbanaInputModel
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

            [Required(ErrorMessage = "El tipo de propiedad es obligatorio")]
            public string Tipo { get; set; }

            public int? Ambientes { get; set; }
            public int? Dormitorios { get; set; }
            public int? Banios { get; set; }

            public int SuperficieTotal { get; set; }
            public int SuperficieCubierta { get; set; }

            public bool Cochera { get; set; }
            public bool Patio { get; set; }
            [DisplayName("Aire Acondicionado")]
            public bool AireAcondicionado { get; set; }
            public bool Seguridad { get; set; }
            public bool Pileta { get; set; }
            public string Direccion { get; set; }
        }
    }
}
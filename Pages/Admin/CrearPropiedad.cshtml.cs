using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

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
            await _context.SaveChangesAsync(); // Guardar para obtener ID

            // Procesar imágenes
            if (Propiedad.Imagenes != null && Propiedad.Imagenes.Count > 0)
            {
                for (int i = 0; i < Propiedad.Imagenes.Count; i++)
                {
                    var imagenFile = Propiedad.Imagenes[i];
                    if (imagenFile.Length > 0)
                    {
                        var imagenPath = await GuardarArchivo(imagenFile, "imagenes");
                        propiedad.Imagenes.Add(new Imagen
                        {
                            Url = imagenPath,
                            EsPrincipal = (i == Propiedad.ImagenPrincipalIndex), // Marcar como principal si coincide con el índice seleccionado
                            PropiedadId = propiedad.Id
                        });
                    }
                }
            }

            // Procesar video de YouTube
            if (!string.IsNullOrEmpty(Propiedad.YoutubeUrl))
            {
                propiedad.Videos.Add(new Video
                {
                    Url = Propiedad.YoutubeUrl,
                    PropiedadId = propiedad.Id
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/GestionPropiedades");
        }

        // Método auxiliar para guardar archivos
        private async Task<string> GuardarArchivo(IFormFile archivo, string subdirectorio)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads", subdirectorio);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            return $"/uploads/{subdirectorio}/{uniqueFileName}";
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

            // Nuevas propiedades para archivos
            [Display(Name = "Imágenes")]
            public List<IFormFile> Imagenes { get; set; } = new List<IFormFile>();

            [Display(Name = "Índice de imagen principal")]
            public int? ImagenPrincipalIndex { get; set; } = 0;

            [Display(Name = "Video (YouTube)")]
            [DataType(DataType.Url)]
            [RegularExpression(@"^$|^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$",
                ErrorMessage = "Debe ser un enlace de YouTube válido")]
            public string? YoutubeUrl { get; set; }
        }
    }
}
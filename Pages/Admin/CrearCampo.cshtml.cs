using GV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

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

            // Procesar im�genes
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
                            EsPrincipal = (i == Propiedad.ImagenPrincipalIndex),
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

            return RedirectToPage("/Admin/GestionCampos");
        }

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

        public class PropiedadCampoInputModel
        {
            [Required(ErrorMessage = "El t�tulo es obligatorio")]
            public string Titulo { get; set; }

            public string Descripcion { get; set; }

            [Required(ErrorMessage = "La ubicaci�n es obligatoria")]
            public string Ubicacion { get; set; }

            [Required(ErrorMessage = "El precio es obligatorio")]
            [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
            public decimal Precio { get; set; }

            public bool EsDestacada { get; set; }

            [Required(ErrorMessage = "La aptitud es obligatoria")]
            public string Aptitud { get; set; }

            [Required(ErrorMessage = "Las hect�reas son obligatorias")]
            [Range(1, int.MaxValue, ErrorMessage = "Las hect�reas deben ser mayor a 0")]
            public int Hectareas { get; set; }

            [Display(Name = "Im�genes")]
            public List<IFormFile> Imagenes { get; set; } = new List<IFormFile>();

            [Display(Name = "�ndice de imagen principal")]
            public int? ImagenPrincipalIndex { get; set; } = 0;

            [Display(Name = "Video (YouTube)")]
            [DataType(DataType.Url)]
            [RegularExpression(@"^$|^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$",
                ErrorMessage = "Debe ser un enlace de YouTube v�lido")]
            public string? YoutubeUrl { get; set; }
        }
    }
}
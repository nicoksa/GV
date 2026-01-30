using GV.Data;
using GV.Models;
using GV.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace GV.Pages.Admin
{
    [Authorize]
    public class CrearCampoModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ImageConversionService _imageService;
        public CrearCampoModel(AppDbContext context, ImageConversionService imageService)
        {
            _context = context;
            _imageService = imageService;
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
            TempData["SuccessMessage"] = "Nuevo campo agregado con éxito";
            return RedirectToPage("/Admin/GestionCampos");
        }

        private async Task<string> GuardarArchivo(IFormFile archivo, string subdirectorio)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads", subdirectorio);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Cambiar extensión a .webp
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(archivo.FileName);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileNameWithoutExtension + ".webp";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Convertir a WebP
            await _imageService.ConvertAndSaveAsync(archivo, filePath, quality: 80);

            return $"/uploads/{subdirectorio}/{uniqueFileName}";
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
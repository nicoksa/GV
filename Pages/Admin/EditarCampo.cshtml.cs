using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GV.Pages.Admin
{
    [Authorize]
    public class EditarCampoModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<EditarCampoModel> _logger;

        public EditarCampoModel(AppDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<EditarCampoModel> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [BindProperty]
        public PropiedadCampoInputModel Propiedad { get; set; }

        public List<ImagenViewModel> ImagenesExistentes { get; set; } = new List<ImagenViewModel>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var propiedad = await _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .Include(p => p.Videos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propiedad == null)
            {
                return NotFound();
            }

            // Mapear a InputModel
            Propiedad = new PropiedadCampoInputModel
            {
                Id = propiedad.Id,
                Titulo = propiedad.Titulo,
                Descripcion = propiedad.Descripcion,
                Ubicacion = propiedad.Ubicacion,
                Precio = propiedad.Precio,
                EsDestacada = propiedad.EsDestacada,
                Aptitud = propiedad.Aptitud,
                Hectareas = propiedad.Hectareas,
                YoutubeUrl = propiedad.Videos.FirstOrDefault()?.Url
            };

            // Preparar imágenes existentes para la vista
            ImagenesExistentes = propiedad.Imagenes
                .Select((img, index) => new ImagenViewModel
                {
                    Id = img.Id,
                    Url = img.Url,
                    EsPrincipal = img.EsPrincipal,
                    Index = index
                }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var propiedadExistente = await _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(p => p.Id == Propiedad.Id);

            if (propiedadExistente == null)
            {
                return NotFound();
            }

            // Actualizar propiedades básicas
            propiedadExistente.Titulo = Propiedad.Titulo;
            propiedadExistente.Descripcion = Propiedad.Descripcion;
            propiedadExistente.Precio = Propiedad.Precio;
            propiedadExistente.Ubicacion = Propiedad.Ubicacion;
            propiedadExistente.EsDestacada = Propiedad.EsDestacada;
            propiedadExistente.Aptitud = Propiedad.Aptitud;
            propiedadExistente.Hectareas = Propiedad.Hectareas;

            // Manejar imágenes nuevas
            if (Propiedad.Imagenes != null && Propiedad.Imagenes.Count > 0)
            {
                foreach (var imagen in Propiedad.Imagenes)
                {
                    if (imagen.Length > 0)
                    {
                        var imagenPath = await GuardarArchivo(imagen, "imagenes");
                        propiedadExistente.Imagenes.Add(new Imagen
                        {
                            Url = imagenPath,
                            EsPrincipal = false, // Se establecerá después
                            PropiedadId = propiedadExistente.Id
                        });
                    }
                }
            }

            // Establecer imagen principal
            if (Propiedad.ImagenPrincipalIndex.HasValue)
            {
                // Si es una imagen nueva
                if (Propiedad.ImagenPrincipalIndex >= propiedadExistente.Imagenes.Count - (Propiedad.Imagenes?.Count ?? 0))
                {
                    var index = Propiedad.ImagenPrincipalIndex.Value - (propiedadExistente.Imagenes.Count - (Propiedad.Imagenes?.Count ?? 0));
                    if (index >= 0 && index < (Propiedad.Imagenes?.Count ?? 0))
                    {
                        var nuevaImagen = propiedadExistente.Imagenes.Last();
                        propiedadExistente.Imagenes.ForEach(img => img.EsPrincipal = false);
                        nuevaImagen.EsPrincipal = true;
                    }
                }
                else // Es una imagen existente
                {
                    if (Propiedad.ImagenPrincipalIndex.Value < propiedadExistente.Imagenes.Count)
                    {
                        propiedadExistente.Imagenes.ForEach(img => img.EsPrincipal = false);
                        propiedadExistente.Imagenes[Propiedad.ImagenPrincipalIndex.Value].EsPrincipal = true;
                    }
                }
            }

            // Manejar video
            if (propiedadExistente.Videos.Any())
            {
                _context.Videos.RemoveRange(propiedadExistente.Videos);
            }

            // Agregar nuevo video solo si se proporciona una URL de YouTube válida
            if (!string.IsNullOrEmpty(Propiedad.YoutubeUrl) &&
                (Propiedad.YoutubeUrl.Contains("youtube.com") || Propiedad.YoutubeUrl.Contains("youtu.be")))
            {
                propiedadExistente.Videos.Add(new Video
                {
                    Url = Propiedad.YoutubeUrl,
                    PropiedadId = propiedadExistente.Id
                });
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Campo editado correctamente";
                return RedirectToPage("/Admin/GestionCampos");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropiedadExists(Propiedad.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }


        public async Task<IActionResult> OnPostDeleteImageAsync(int id, int imageId)
        {
            var propiedad = await _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propiedad == null) return NotFound();

            var imagen = propiedad.Imagenes.FirstOrDefault(i => i.Id == imageId);
            if (imagen == null) return NotFound();

            // Eliminar archivo físico
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, imagen.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Eliminar de la base de datos
            _context.Imagenes.Remove(imagen);
            await _context.SaveChangesAsync();

            // Actualizar imagen principal si era la principal
            if (imagen.EsPrincipal && propiedad.Imagenes.Any())
            {
                propiedad.Imagenes.First().EsPrincipal = true;
                await _context.SaveChangesAsync();
            }

            // Devolver éxito sin recargar
            return new OkResult();
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

        private bool PropiedadExists(int id)
        {
            return _context.PropiedadesCampo.Any(e => e.Id == id);
        }

        public class PropiedadCampoInputModel
        {
            public int Id { get; set; }

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
            public int? ImagenPrincipalIndex { get; set; }

            [Display(Name = "Video (YouTube)")]
            [DataType(DataType.Url)]
            [RegularExpression(@"^$|^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$",
                ErrorMessage = "Debe ser un enlace de YouTube válido")]
            public string? YoutubeUrl { get; set; }
        }

        public class ImagenViewModel
        {
            public int Id { get; set; }
            public string Url { get; set; }
            public bool EsPrincipal { get; set; }
            public int Index { get; set; }
        }
    }
}
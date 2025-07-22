using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace GV.Pages.Admin
{
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

        public async Task<IActionResult> OnPostAsync(IFormFileCollection imagenes, string videoUrl)
        {

            _logger.LogInformation($"Intentando guardar {imagenes?.Count ?? 0} imágenes");
            foreach (var imagen in imagenes)
            {
                _logger.LogInformation($"Procesando imagen: {imagen.FileName}, tamaño: {imagen.Length}");
            }


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
            if (imagenes != null && imagenes.Count > 0)
            {
                // Verificar si hay imágenes existentes para determinar si la nueva será principal
                bool esPrimeraImagen = !propiedadExistente.Imagenes.Any();

                foreach (var imagen in imagenes)
                {
                    if (imagen.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/campos");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagen.CopyToAsync(fileStream);
                        }

                        var nuevaImagen = new Imagen
                        {
                            Url = Path.Combine("images/campos", uniqueFileName).Replace("\\", "/"),
                            EsPrincipal = esPrimeraImagen, // Solo será principal si es la primera imagen
                            PropiedadId = propiedadExistente.Id
                        };

                        propiedadExistente.Imagenes.Add(nuevaImagen);
                        esPrimeraImagen = false; // Las siguientes imágenes no serán principales
                    }
                }
            }

            // Manejar video
            if (!string.IsNullOrEmpty(videoUrl))
            {
                videoUrl = videoUrl.Trim();
                // Eliminar video existente si hay uno
                if (propiedadExistente.Videos.Any())
                {
                    _context.Videos.RemoveRange(propiedadExistente.Videos);
                }

                // Agregar nuevo video solo si es de YouTube
                if (videoUrl.Contains("youtube.com") || videoUrl.Contains("youtu.be"))
                {
                    propiedadExistente.Videos.Add(new Video
                    {
                        Url = videoUrl,
                        PropiedadId = propiedadExistente.Id
                    });
                }
            }

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
                throw;
            }
        }

        public async Task<IActionResult> OnPostDeleteImageAsync(int id, int imageId)
        {
            var propiedad = await _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propiedad == null)
            {
                return NotFound();
            }

            var imagen = propiedad.Imagenes.FirstOrDefault(i => i.Id == imageId);
            if (imagen != null)
            {
                // Eliminar archivo físico
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, imagen.Url);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Eliminar de la base de datos
                _context.Imagenes.Remove(imagen);
                await _context.SaveChangesAsync();

                // Si era la imagen principal y quedan imágenes, establecer la primera como principal
                if (imagen.EsPrincipal && propiedad.Imagenes.Any())
                {
                    propiedad.Imagenes.First().EsPrincipal = true;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostSetMainImageAsync(int id, int imageId)
        {
            var propiedad = await _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propiedad == null)
            {
                return NotFound();
            }

            foreach (var img in propiedad.Imagenes)
            {
                img.EsPrincipal = (img.Id == imageId);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }

        private bool PropiedadExists(int id)
        {
            return _context.PropiedadesCampo.Any(e => e.Id == id);
        }
    }
}
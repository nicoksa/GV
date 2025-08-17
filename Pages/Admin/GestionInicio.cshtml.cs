using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;                                  
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System; 

namespace GV.Pages.Admin
{
    public class GestionInicioModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public GestionInicioModel(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public List<SliderImage> SliderImages { get; set; } = new List<SliderImage>();

        [BindProperty]
        public IFormFileCollection NewImages { get; set; }

        public async Task OnGetAsync()
        {
            await LoadSliderImages();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewImages != null && NewImages.Count > 0)
            {
                foreach (var image in NewImages)
                {
                    if (image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "fondos", "slider");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        // Guardar en base de datos si es necesario
                        // O simplemente mantener referencia en un archivo de configuración
                    }
                }

                TempData["SuccessMessage"] = "Imágenes subidas correctamente";
                return RedirectToPage();
            }

            TempData["ErrorMessage"] = "Por favor seleccione al menos una imagen";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteImageAsync(string imageName)
        {
            var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "fondos", "slider", imageName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
                TempData["SuccessMessage"] = "Imagen eliminada correctamente";
            }
            else
            {
                TempData["ErrorMessage"] = "La imagen no existe";
            }

            await LoadSliderImages();
            return RedirectToPage();
        }

        private async Task LoadSliderImages()
        {
            var imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "fondos", "slider");
            if (Directory.Exists(imagesFolder))
            {
                var imageFiles = Directory.GetFiles(imagesFolder)
                    .Select(f => new SliderImage
                    {
                        FileName = Path.GetFileName(f),
                        Path = f.Replace(_hostingEnvironment.WebRootPath, "").Replace("\\", "/")
                    })
                    .ToList();

                SliderImages = imageFiles;
            }
        }
    }

    public class SliderImage
    {
        public string FileName { get; set; }
        public string Path { get; set; }
    }
}

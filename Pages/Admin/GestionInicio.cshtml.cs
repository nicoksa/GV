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
using System.Text.Json; // Añade este using para JsonSerializer

namespace GV.Pages.Admin
{
    public class GestionInicioModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _orderConfigPath;

        public GestionInicioModel(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _orderConfigPath = Path.Combine(hostingEnvironment.ContentRootPath, "slider-order.json");
        }

        [BindProperty]
        public List<SliderImage> SliderImages { get; set; } = new List<SliderImage>();

        [BindProperty]
        public IFormFileCollection NewImages { get; set; }

        public async Task OnGetAsync()
        {
            await LoadSliderImages();
        }

        // Método para obtener el orden de las imágenes desde un archivo JSON
        private Dictionary<string, int> GetImageOrder()
        {
            if (!System.IO.File.Exists(_orderConfigPath))
            {
                return new Dictionary<string, int>();
            }

            try
            {
                var json = System.IO.File.ReadAllText(_orderConfigPath);
                return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
            }
            catch
            {
                return new Dictionary<string, int>();
            }
        }

        // Método para guardar el orden de las imágenes en un archivo JSON
        private async Task SaveImageOrder(Dictionary<string, int> orderConfig)
        {
            try
            {
                var json = JsonSerializer.Serialize(orderConfig);
                await System.IO.File.WriteAllTextAsync(_orderConfigPath, json);
            }
            catch (Exception ex)
            {
                // Puedes loggear el error si lo necesitas
                Console.WriteLine($"Error al guardar el orden: {ex.Message}");
            }
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

                // Eliminar también del orden configurado
                var orderConfig = GetImageOrder();
                if (orderConfig.ContainsKey(imageName))
                {
                    orderConfig.Remove(imageName);
                    await SaveImageOrder(orderConfig);
                }

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
                var orderConfig = GetImageOrder();

                var imageFiles = Directory.GetFiles(imagesFolder)
                    .Select(f => new SliderImage
                    {
                        FileName = Path.GetFileName(f),
                        Path = f.Replace(_hostingEnvironment.WebRootPath, "").Replace("\\", "/"),
                        Order = orderConfig.TryGetValue(Path.GetFileName(f), out var order) ? order : 0
                    })
                    .OrderBy(x => x.Order)
                    .ToList();

                SliderImages = imageFiles;
            }
        }

        public async Task<IActionResult> OnPostSaveImageOrderAsync([FromBody] List<ImageOrder> imageOrder)
        {
            var orderConfig = imageOrder.ToDictionary(x => x.FileName, x => x.Order);
            await SaveImageOrder(orderConfig);
            return new OkResult();
        }

        public class ImageOrder
        {
            public string FileName { get; set; }
            public int Order { get; set; }
        }
    }

    public class SliderImage
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public int Order { get; set; }
    }
}

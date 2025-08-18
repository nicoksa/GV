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
using System.Text.Json; // Añade este using

namespace GV.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _orderConfigPath;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _orderConfigPath = Path.Combine(hostingEnvironment.ContentRootPath, "slider-order.json");
        }

        public List<PropiedadUrbana> PropiedadesDestacadas { get; set; } = new List<PropiedadUrbana>();
        public List<PropiedadCampo> CamposDestacados { get; set; } = new List<PropiedadCampo>();
        public List<string> SliderImages { get; set; } = new List<string>();

        // Método para obtener el orden de las imágenes
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

        public void OnGet()
        {
            ViewData["IsIndexPage"] = true;

            // Obtener propiedades urbanas destacadas con sus imágenes
            PropiedadesDestacadas = _context.PropiedadesUrbanas
                .Include(p => p.Imagenes)
                .Where(p => p.EsDestacada)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();

            // Obtener campos destacados con sus imágenes
            CamposDestacados = _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .Where(p => p.EsDestacada)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();

            // Cargar imágenes del slider con ordenamiento
            var sliderFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "fondos", "slider");
            if (Directory.Exists(sliderFolder))
            {
                var orderConfig = GetImageOrder();

                SliderImages = Directory.GetFiles(sliderFolder)
                    .Select(file => new {
                        Path = $"/images/fondos/slider/{Path.GetFileName(file)}",
                        Order = orderConfig.TryGetValue(Path.GetFileName(file), out var order) ? order : 0,
                        CreationTime = new FileInfo(file).CreationTime
                    })
                    .OrderBy(x => x.Order) // Ordenar primero por el orden configurado
                    .ThenByDescending(x => x.CreationTime) // Luego por fecha de creación (nuevas primero)
                    .Select(x => x.Path)
                    .ToList();
            }
        }
    }
}
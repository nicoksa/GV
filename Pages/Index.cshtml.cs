using GV.Data;
using GV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GV.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public List<PropiedadUrbana> PropiedadesDestacadas { get; set; } = new List<PropiedadUrbana>();
        public List<PropiedadCampo> CamposDestacados { get; set; } = new List<PropiedadCampo>();
        public List<string> SliderImages { get; set; } = new List<string>();

        public void OnGet()
        {
            // Propiedad para identificar que estamos en el Index
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

            // Cargar imágenes del slider
            var sliderFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "fondos", "slider");
            if (Directory.Exists(sliderFolder))
            {
                SliderImages = Directory.GetFiles(sliderFolder)
                     .Select(file => new {
                         Path = $"/images/fondos/slider/{Path.GetFileName(file)}",
                         CreationTime = new FileInfo(file).CreationTime
                     })
                     .OrderByDescending(x => x.CreationTime) // Más nuevas primero
                                                             //.OrderBy(x => x.CreationTime) // Más antiguas primero
                     .Select(x => x.Path)
                     .ToList();
            }
        }
    }
}
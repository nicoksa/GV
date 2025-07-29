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

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<PropiedadUrbana> PropiedadesDestacadas { get; set; } = new List<PropiedadUrbana>();
        public List<PropiedadCampo> CamposDestacados { get; set; } = new List<PropiedadCampo>();

        public void OnGet()
        {
            // Propiedad para identificar que estamos en el Index
            ViewData["IsIndexPage"] = true;

            // Obtener propiedades urbanas destacadas (máximo 3) con sus imágenes
            PropiedadesDestacadas = _context.PropiedadesUrbanas
                .Include(p => p.Imagenes)
                .Where(p => p.EsDestacada)
                .OrderByDescending(p => p.FechaPublicacion)
                .Take(3)
                .ToList();

            // Obtener campos destacados (máximo 3) con sus imágenes
            CamposDestacados = _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .Where(p => p.EsDestacada)
                .OrderByDescending(p => p.FechaPublicacion)
                .Take(3)
                .ToList();
        }
    }
}
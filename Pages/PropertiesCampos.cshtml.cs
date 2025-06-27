using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using GV.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GV.Pages
{
    public class PropertiesCamposModel : PageModel
    {
        private readonly AppDbContext _context;

        public PropertiesCamposModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)] public string? Aptitud { get; set; }
        [BindProperty(SupportsGet = true)] public string? Ciudad { get; set; }
        [BindProperty(SupportsGet = true)] public string? Superficie { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? PrecioMin { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? PrecioMax { get; set; }

        // Propiedades para paginación
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        public List<PropiedadCampo> Resultados { get; set; } = new();

        public void OnGet()
        {
            var query = _context.PropiedadesCampo
                .Include(p => p.Imagenes)
                .AsQueryable();

            // Filtro Tipo (Aptitud en PropiedadCampo)
            if (!string.IsNullOrWhiteSpace(Aptitud))
                query = query.Where(p => p.Aptitud == Aptitud);

            // Filtro Ubicación
            if (!string.IsNullOrWhiteSpace(Ciudad))
                query = query.Where(p => p.Ubicacion == Ciudad);

            // Filtro Superficie (Hectareas en PropiedadCampo)
            if (!string.IsNullOrWhiteSpace(Superficie))
            {
                switch (Superficie)
                {
                    case "Menos de 50 Ha":
                        query = query.Where(p => p.Hectareas < 50);
                        break;
                    case "50 - 100 Ha":
                        query = query.Where(p => p.Hectareas >= 50 && p.Hectareas <= 100);
                        break;
                    case "100 - 200 Ha":
                        query = query.Where(p => p.Hectareas > 100 && p.Hectareas <= 200);
                        break;
                    case "200 - 500 Ha":
                        query = query.Where(p => p.Hectareas > 200 && p.Hectareas <= 500);
                        break;
                    case "Más de 500 Ha":
                        query = query.Where(p => p.Hectareas > 500);
                        break;
                }
            }

            // Filtros Precio
            if (PrecioMin.HasValue)
                query = query.Where(p => p.Precio >= PrecioMin.Value);

            if (PrecioMax.HasValue)
                query = query.Where(p => p.Precio <= PrecioMax.Value);

            // Obtener el conteo total antes de paginar
            TotalItems = query.Count();

            // Aplicar paginación
            Resultados = query
                .OrderBy(p => p.Id)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
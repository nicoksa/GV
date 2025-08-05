using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using GV.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GV.Pages
{
    public class PropertiesCamposModel : PageModel
    {
        private readonly AppDbContext _context;

        public PropertiesCamposModel(AppDbContext context)
        {
            _context = context;
        }

        // Propiedades para filtros
        [BindProperty(SupportsGet = true)] public string? Aptitud { get; set; }
        [BindProperty(SupportsGet = true)] public string? Ciudad { get; set; }
        [BindProperty(SupportsGet = true)] public string? Superficie { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? PrecioMin { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? PrecioMax { get; set; }

        // Propiedad para ordenamiento
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "";

        // Propiedades para paginación
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        public List<PropiedadCampo> Resultados { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                // Configurar consulta base con tracking desactivado para mejor performance
                var query = _context.PropiedadesCampo
                    .Include(p => p.Imagenes)
                    .AsNoTracking()
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(Aptitud))
                    query = query.Where(p => p.Aptitud == Aptitud);

                if (!string.IsNullOrWhiteSpace(Ciudad))
                    query = query.Where(p => p.Ubicacion == Ciudad);

                // Filtro por superficie
                if (!string.IsNullOrWhiteSpace(Superficie))
                {
                    query = Superficie switch
                    {
                        "Menos de 50 Ha" => query.Where(p => p.Hectareas < 50),
                        "50 - 100 Ha" => query.Where(p => p.Hectareas >= 50 && p.Hectareas <= 100),
                        "100 - 200 Ha" => query.Where(p => p.Hectareas > 100 && p.Hectareas <= 200),
                        "200 - 500 Ha" => query.Where(p => p.Hectareas > 200 && p.Hectareas <= 500),
                        "Más de 500 Ha" => query.Where(p => p.Hectareas > 500),
                        _ => query
                    };
                }

                // Filtros por precio
                if (PrecioMin.HasValue)
                    query = query.Where(p => p.Precio >= PrecioMin.Value);

                if (PrecioMax.HasValue)
                    query = query.Where(p => p.Precio <= PrecioMax.Value);

                // Aplicar ordenamiento
                query = SortOrder switch
                {
                    "precio_asc" => query.OrderBy(p => p.Precio),
                    "precio_desc" => query.OrderByDescending(p => p.Precio),
                    "hectareas_asc" => query.OrderBy(p => p.Hectareas),
                    "hectareas_desc" => query.OrderByDescending(p => p.Hectareas),
                    _ => query.OrderByDescending(p => p.Id) // Orden por defecto
                };

                // Obtener conteo total (optimizado)
                TotalItems = await query.CountAsync();

                // Aplicar paginación
                Resultados = await query
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Manejo básico de errores
                Resultados = new List<PropiedadCampo>();
                TotalItems = 0;
                // Considera registrar el error (ex) en un sistema de logging
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GV.Data;
using GV.Models;
using Microsoft.EntityFrameworkCore;

namespace GV.Pages
{
    public class PropertiesUrbanoModel : PageModel
    {
        private readonly AppDbContext _context;

        public PropertiesUrbanoModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)] public string? Tipo { get; set; }
        [BindProperty(SupportsGet = true)] public int? Ambientes { get; set; }
        [BindProperty(SupportsGet = true)] public int? Dormitorios { get; set; }
        [BindProperty(SupportsGet = true)] public string? Ubicacion { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? PrecioMin { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? PrecioMax { get; set; }

        [BindProperty(SupportsGet = true)] public int? Banios { get; set; }


        // Propiedad para ordenamiento
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "";

        // Propiedades para paginación
        [BindProperty(SupportsGet = true)]
      
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12; // 9 cards por página
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        


        public List<PropiedadUrbana> Resultados { get; set; } = new();


        public async Task OnGetAsync()
        {
            try
            {
                var query = _context.PropiedadesUrbanas
                    .Include(p => p.Imagenes)
                    .AsQueryable();

                // Filtros (mantén tus filtros actuales)
                if (!string.IsNullOrWhiteSpace(Tipo))
                    query = query.Where(p => p.Tipo == Tipo);

                if (!string.IsNullOrWhiteSpace(Ubicacion))
                    query = query.Where(p => p.Ubicacion == Ubicacion);

                if (Ambientes.HasValue)
                    query = query.Where(p => Ambientes == 4 ? p.Ambientes >= 4 : p.Ambientes == Ambientes.Value);

                if (Dormitorios.HasValue)
                    query = query.Where(p => Dormitorios == 4 ? p.Dormitorios >= 4 : p.Dormitorios == Dormitorios.Value);

                if (Banios.HasValue)
                    query = query.Where(p => Banios == 4 ? p.Banios >= 4 : p.Banios == Banios.Value);

                if (PrecioMin.HasValue)
                    query = query.Where(p => p.Precio >= PrecioMin.Value);

                if (PrecioMax.HasValue)
                    query = query.Where(p => p.Precio <= PrecioMax.Value);

                // Aplicar ordenamiento
                query = SortOrder switch
                {
                    "precio_asc" => query.OrderBy(p => p.Precio),
                    "precio_desc" => query.OrderByDescending(p => p.Precio),
                    "dormitorios_asc" => query.OrderBy(p => p.Dormitorios),
                    "dormitorios_desc" => query.OrderByDescending(p => p.Dormitorios),
                    _ => query.OrderByDescending(p => p.Id) // Orden por defecto
                };

                // Obtener conteo total
                TotalItems = await query.CountAsync();

                // Aplicar paginación (sin volver a ordenar)
                Resultados = await query
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Resultados = new List<PropiedadUrbana>();
                TotalItems = 0;
                // Loggear el error
            }
        }
    }
}

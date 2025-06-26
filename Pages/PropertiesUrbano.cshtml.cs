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




        // Propiedades para paginaci�n
        [BindProperty(SupportsGet = true)]
      
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9; // 9 cards por p�gina
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        


        public List<PropiedadUrbana> Resultados { get; set; } = new();


        public void OnGet()
        {
            var query = _context.PropiedadesUrbanas
         .Include(p => p.Imagenes)
         .AsQueryable();

            // Filtro Tipo - exact match
            if (!string.IsNullOrWhiteSpace(Tipo))
                query = query.Where(p => p.Tipo == Tipo);

            // Filtro Ubicaci�n - exact match (cambiar Contains por ==)
            if (!string.IsNullOrWhiteSpace(Ubicacion))
                query = query.Where(p => p.Ubicacion == Ubicacion);

            // Filtro Ambientes
            if (Ambientes.HasValue)
            {
                if (Ambientes == 4) // Para "4+"
                    query = query.Where(p => p.Ambientes >= 4);
                else
                    query = query.Where(p => p.Ambientes == Ambientes.Value);
            }

            // Filtro Dormitorios
            if (Dormitorios.HasValue)
            {
                if (Dormitorios == 4) // Para "4+"
                    query = query.Where(p => p.Dormitorios >= 4);
                else
                    query = query.Where(p => p.Dormitorios == Dormitorios.Value);
            }

            // Filtro Ba�os
            if (Banios.HasValue)
            {
                if (Banios == 4) // Para "4+"
                    query = query.Where(p => p.Banios >= 4);
                else
                    query = query.Where(p => p.Banios == Banios.Value);
            }

            // Filtros Precio
            if (PrecioMin.HasValue)
                query = query.Where(p => p.Precio >= PrecioMin.Value);

            if (PrecioMax.HasValue)
                query = query.Where(p => p.Precio <= PrecioMax.Value);

            // Obtener el conteo total antes de paginar
            TotalItems = query.Count();

            // Aplicar paginaci�n
            Resultados = query
                .OrderBy(p => p.Id) // Ordenar por alg�n campo, se puede cambiar
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();


            Console.WriteLine($"Resultados encontrados: {TotalItems}");
            Console.WriteLine($"Mostrando {Resultados.Count} en p�gina {CurrentPage} de {TotalPages}");
        }
    }
}

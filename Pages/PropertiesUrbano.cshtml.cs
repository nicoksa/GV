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
        [BindProperty(SupportsGet = true)] public string? Ubicacion { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? PrecioMin { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? PrecioMax { get; set; }

        [BindProperty(SupportsGet = true)] public int? Banios { get; set; }

        public List<PropiedadUrbana> Resultados { get; set; } = new();


        public void OnGet()
        {
            var query = _context.PropiedadesUrbanas
                .Include(p => p.Imagenes)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Tipo))
                query = query.Where(p => p.Tipo == Tipo);

            if (!string.IsNullOrWhiteSpace(Ubicacion))
                query = query.Where(p => p.Ubicacion.Contains(Ubicacion));

            if (Ambientes.HasValue)
                query = query.Where(p => p.Ambientes >= Ambientes.Value);

            if (PrecioMin.HasValue)
                query = query.Where(p => p.Precio >= PrecioMin.Value);

            if (PrecioMax.HasValue)
                query = query.Where(p => p.Precio <= PrecioMax.Value);

            if (Banios.HasValue)
                query = query.Where(p => p.Banios >= Banios.Value);

            Resultados = query.ToList();


            Console.WriteLine($"Resultados encontrados: {Resultados.Count}");
            foreach (var p in Resultados)
            {
                Console.WriteLine($"Propiedad: {p.Titulo}, Imágenes: {p.Imagenes?.Count ?? 0}");
            }
        }
    }
}

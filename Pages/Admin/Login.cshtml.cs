using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GV.Pages.Admin
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El email es obligatorio")]
            [EmailAddress(ErrorMessage = "Debe ser un email v�lido")]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contrase�a es obligatoria")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Aqu� ir�a tu l�gica de autenticaci�n
            // Por ahora solo redireccionamos si es v�lido
            return RedirectToPage("/Index");
        }
    }
}
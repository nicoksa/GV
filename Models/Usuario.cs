namespace GV.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; } // Hasheado
        public bool EsAdmin { get; set; } = true;
    }
}

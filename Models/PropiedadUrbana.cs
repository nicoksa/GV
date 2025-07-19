using System.ComponentModel;

namespace GV.Models
{
    public class PropiedadUrbana : Propiedad
    {


        public string Tipo { get; set; }

        [DisplayName("Ambientes")]
        public int? Ambientes { get; set; }

        [DisplayName("Dormitorios")]
        public int? Dormitorios { get; set; }

        [DisplayName("Baños")]
        public int? Banios { get; set; }

        [DisplayName("Superficie Total")]
        public int SuperficieTotal { get; set; }

        [DisplayName("Superficie Cubierta")]
        public int SuperficieCubierta { get; set; }
        public bool Cochera { get; set; }
        public bool Patio { get; set; }

        [DisplayName("Aire Acondicionado")]
        public bool Aire_acond { get; set; }
        public bool Seguridad{ get; set; }
        public bool Pileta { get; set; }
        public string Direccion { get; set; }


        public PropiedadUrbana()
        {
            Division = "Urbano";
            Imagenes = new List<Imagen>();
            Videos = new List<Video>();
        }
    }
}

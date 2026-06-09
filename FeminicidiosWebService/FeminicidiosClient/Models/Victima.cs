using System.Xml.Serialization;

namespace FeminicidiosClient.Models
{
    public class Victima
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string FechaNacimiento { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string NombreVictimario { get; set; } = string.Empty;
        public string ApellidoVictimario { get; set; } = string.Empty;
        public string Relacion { get; set; } = string.Empty;
        public string FechaNacimientoVictimario { get; set; } = string.Empty;
        public string SexoVictimario { get; set; } = string.Empty;
        public int IdProvincia { get; set; }
    }
}

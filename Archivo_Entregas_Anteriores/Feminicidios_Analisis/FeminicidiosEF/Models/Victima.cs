using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeminicidiosEF.Models
{
    [Table("tblVictimas")]
    public class Victima
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string TipoDocumento { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Documento { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        public string FechaNacimiento { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string Sexo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string NombreVictimario { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ApellidoVictimario { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Relacion { get; set; } = string.Empty;

        [Required]
        public string FechaNacimientoVictimario { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string SexoVictimario { get; set; } = string.Empty;

        // Clave foranea
        public int IdProvincia { get; set; }

        [ForeignKey("IdProvincia")]
        public Provincia? Provincia { get; set; }
    }
}

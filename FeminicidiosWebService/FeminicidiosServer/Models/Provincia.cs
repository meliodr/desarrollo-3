using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeminicidiosServer.Models
{
    [Table("tblProvincias")]
    public class Provincia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public int Cantidad { get; set; } = 0;

        [Required]
        public string FechaUltimaActualizacion { get; set; } = string.Empty;

        // Relacion con Victimas
        public ICollection<Victima> Victimas { get; set; } = new List<Victima>();
    }
}

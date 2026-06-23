namespace MundialRegistro.Models;

public class Partido
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Hora { get; set; } = string.Empty;
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
    public string Estadio { get; set; } = string.Empty;
    public int TotalAsientos { get; set; } = 72;

    public ICollection<Registro> Registros { get; set; } = new List<Registro>();
}

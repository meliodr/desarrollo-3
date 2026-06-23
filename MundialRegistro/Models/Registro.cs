namespace MundialRegistro.Models;

public class Registro
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string Asiento { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }

    public int PartidoId { get; set; }
    public Partido? Partido { get; set; }
}

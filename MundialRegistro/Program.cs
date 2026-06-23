using Microsoft.EntityFrameworkCore;
using MundialRegistro.Data;
using MundialRegistro.Models;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "mundial_registro.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Partidos.Any())
    {
        db.Partidos.AddRange(
            new Partido { Fecha = new DateTime(2026, 6, 11), Hora = "19:00", EquipoLocal = "México", EquipoVisitante = "Canadá", Estadio = "Estadio Azteca", TotalAsientos = 72 },
            new Partido { Fecha = new DateTime(2026, 6, 12), Hora = "16:00", EquipoLocal = "Estados Unidos", EquipoVisitante = "Gales", Estadio = "SoFi Stadium", TotalAsientos = 72 },
            new Partido { Fecha = new DateTime(2026, 6, 12), Hora = "20:00", EquipoLocal = "Argentina", EquipoVisitante = "Japón", Estadio = "MetLife Stadium", TotalAsientos = 72 },
            new Partido { Fecha = new DateTime(2026, 6, 13), Hora = "17:00", EquipoLocal = "Brasil", EquipoVisitante = "Marruecos", Estadio = "Hard Rock Stadium", TotalAsientos = 72 },
            new Partido { Fecha = new DateTime(2026, 6, 13), Hora = "21:00", EquipoLocal = "España", EquipoVisitante = "Senegal", Estadio = "AT&T Stadium", TotalAsientos = 72 },
            new Partido { Fecha = new DateTime(2026, 6, 14), Hora = "18:00", EquipoLocal = "Francia", EquipoVisitante = "Uruguay", Estadio = "BC Place", TotalAsientos = 72 },
            new Partido { Fecha = new DateTime(2026, 6, 14), Hora = "22:00", EquipoLocal = "Alemania", EquipoVisitante = "Corea del Sur", Estadio = "Levi's Stadium", TotalAsientos = 72 }
        );
        db.SaveChanges();
    }
}

app.MapGet("/api/partidos", async (AppDbContext db) =>
{
    var partidos = await db.Partidos
        .Include(p => p.Registros)
        .OrderBy(p => p.Fecha)
        .ThenBy(p => p.Hora)
        .ToListAsync();

    return Results.Ok(partidos.Select(p => new
    {
        p.Id,
        Fecha = p.Fecha.ToString("yyyy-MM-dd"),
        FechaTexto = p.Fecha.ToString("dd/MM/yyyy"),
        p.Hora,
        p.EquipoLocal,
        p.EquipoVisitante,
        p.Estadio,
        p.TotalAsientos,
        AsientosOcupados = p.Registros.Count
    }));
});

app.MapGet("/api/partidos/{id:int}/asientos", async (int id, AppDbContext db) =>
{
    var partido = await db.Partidos.FindAsync(id);
    if (partido is null)
    {
        return Results.NotFound(new { mensaje = "Partido no encontrado." });
    }

    var ocupados = await db.Registros
        .Where(r => r.PartidoId == id)
        .Select(r => r.Asiento)
        .OrderBy(a => a)
        .ToListAsync();

    return Results.Ok(new { partidoId = id, asientosOcupados = ocupados });
});

app.MapPost("/api/registros", async (RegistroRequest request, AppDbContext db) =>
{
    var errores = ValidarRegistro(request);
    if (errores.Count > 0)
    {
        return Results.BadRequest(new { errores });
    }

    var partido = await db.Partidos.FindAsync(request.PartidoId);
    if (partido is null)
    {
        return Results.NotFound(new { mensaje = "Debe seleccionar un partido válido." });
    }

    var asiento = request.Asiento!.Trim().ToUpperInvariant();
    var asientoOcupado = await db.Registros.AnyAsync(r => r.PartidoId == request.PartidoId && r.Asiento == asiento);
    if (asientoOcupado)
    {
        return Results.Conflict(new { mensaje = "Este asiento ya fue seleccionado. Elija otro asiento disponible." });
    }

    var registro = new Registro
    {
        Codigo = CrearCodigoComprobante(),
        TipoDocumento = request.TipoDocumento!.Trim(),
        Documento = request.Documento!.Trim(),
        Nombres = request.Nombres!.Trim(),
        Apellidos = request.Apellidos!.Trim(),
        FechaNacimiento = request.FechaNacimiento.Date,
        Sexo = request.Sexo!.Trim(),
        PartidoId = request.PartidoId,
        Asiento = asiento,
        FechaRegistro = DateTime.Now
    };

    db.Registros.Add(registro);
    await db.SaveChangesAsync();

    return Results.Created($"/api/registros/{registro.Codigo}", CrearComprobante(registro, partido));
});

app.MapGet("/api/registros/{codigo}", async (string codigo, AppDbContext db) =>
{
    var registro = await db.Registros
        .Include(r => r.Partido)
        .FirstOrDefaultAsync(r => r.Codigo == codigo.Trim().ToUpperInvariant());

    if (registro is null || registro.Partido is null)
    {
        return Results.NotFound(new { mensaje = "Comprobante no encontrado." });
    }

    return Results.Ok(CrearComprobante(registro, registro.Partido));
});

app.MapFallbackToFile("index.html");

app.Run();

static List<string> ValidarRegistro(RegistroRequest request)
{
    var errores = new List<string>();

    if (string.IsNullOrWhiteSpace(request.TipoDocumento)) errores.Add("Seleccione el tipo de documento.");
    if (string.IsNullOrWhiteSpace(request.Documento)) errores.Add("Digite el número de documento.");
    if (string.IsNullOrWhiteSpace(request.Nombres)) errores.Add("Digite los nombres.");
    if (string.IsNullOrWhiteSpace(request.Apellidos)) errores.Add("Digite los apellidos.");
    if (request.FechaNacimiento == default) errores.Add("Seleccione la fecha de nacimiento.");
    if (string.IsNullOrWhiteSpace(request.Sexo)) errores.Add("Seleccione el sexo.");
    if (request.PartidoId <= 0) errores.Add("Seleccione un partido.");
    if (string.IsNullOrWhiteSpace(request.Asiento)) errores.Add("Seleccione un asiento.");

    if (request.FechaNacimiento != default && request.FechaNacimiento.Date >= DateTime.Today)
    {
        errores.Add("La fecha de nacimiento debe ser menor a la fecha actual.");
    }

    return errores;
}

static string CrearCodigoComprobante()
{
    return $"WM-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
}

static object CrearComprobante(Registro registro, Partido partido)
{
    return new
    {
        registro.Id,
        registro.Codigo,
        FechaRegistro = registro.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
        Persona = new
        {
            registro.TipoDocumento,
            registro.Documento,
            registro.Nombres,
            registro.Apellidos,
            FechaNacimiento = registro.FechaNacimiento.ToString("dd/MM/yyyy"),
            registro.Sexo
        },
        Partido = new
        {
            partido.Id,
            Fecha = partido.Fecha.ToString("dd/MM/yyyy"),
            partido.Hora,
            partido.EquipoLocal,
            partido.EquipoVisitante,
            partido.Estadio
        },
        registro.Asiento
    };
}

public record RegistroRequest(
    string? TipoDocumento,
    string? Documento,
    string? Nombres,
    string? Apellidos,
    DateTime FechaNacimiento,
    string? Sexo,
    int PartidoId,
    string? Asiento);

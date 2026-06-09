/*
 * ============================================================
 * INTEC - Laboratorio de Desarrollo 3
 * Aplicacion: Registro de Feminicidios - Entity Framework
 * Alumno: Jarry Gonzalez  |  ID: 1127998
 * Maestro: Carlos Mendez
 * ============================================================
 */

using FeminicidiosEF.Data;
using FeminicidiosEF.Models;
using FeminicidiosEF.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

Console.WriteLine("=================================================");
Console.WriteLine("  REGISTRO DE FEMINICIDIOS POR PROVINCIA");
Console.WriteLine("  INTEC - Laboratorio de Desarrollo 3");
Console.WriteLine("  Entity Framework + EventLog");
Console.WriteLine("  Alumno: Jarry Gonzalez | ID: 1127998");
Console.WriteLine("=================================================\n");

// Crear/migrar base de datos automaticamente
using var db = new AppDbContext();
db.Database.EnsureCreated();
Console.WriteLine("Base de datos lista.\n");
EventLogger.Registrar("Sistema iniciado correctamente.");

int opcion = 0;
do
{
    Console.WriteLine("\n--- MENU PRINCIPAL ---");
    Console.WriteLine("1. Registrar / Actualizar Provincia  (SP Upsert)");
    Console.WriteLine("2. Registrar Victima                 (SP Insert + Transaccion)");
    Console.WriteLine("3. Listar Provincias");
    Console.WriteLine("4. Listar Victimas");
    Console.WriteLine("5. Salir");
    Console.Write("Seleccione una opcion: ");
    int.TryParse(Console.ReadLine(), out opcion);

    switch (opcion)
    {
        case 1: await SpUpsertProvincia(db);  break;
        case 2: await SpInsertVictima(db);    break;
        case 3: ListarProvincias(db);         break;
        case 4: ListarVictimas(db);           break;
        case 5: Console.WriteLine("\nCerrando sistema..."); break;
        default: Console.WriteLine("Opcion invalida."); break;
    }
} while (opcion != 5);

EventLogger.Registrar("Sistema cerrado correctamente.");

// ============================================================
//  SP_UPSERT_PROVINCIA
//  Inserta la provincia si no existe; actualiza si ya existe.
// ============================================================
static async Task SpUpsertProvincia(AppDbContext db)
{
    Console.WriteLine("\n--- SP: UPSERT PROVINCIA ---");
    Console.Write("Nombre de la Provincia: ");
    string nombre = Console.ReadLine() ?? "";
    Console.Write("Fecha (YYYY-MM-DD): ");
    string fecha = Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd");

    try
    {
        // Buscar si ya existe
        var existente = await db.Provincias
            .FirstOrDefaultAsync(p => p.Nombre.ToLower() == nombre.ToLower());

        if (existente != null)
        {
            // UPDATE: solo actualiza la fecha
            existente.FechaUltimaActualizacion = fecha;
            db.Provincias.Update(existente);
            await db.SaveChangesAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[OK] Provincia '{nombre}' actualizada (fecha: {fecha}).");
            Console.ResetColor();
            EventLogger.Registrar($"SP_Upsert_Provincia: Provincia '{nombre}' actualizada.");
        }
        else
        {
            // INSERT: nueva provincia
            var nueva = new Provincia
            {
                Nombre                   = nombre,
                Cantidad                 = 0,
                FechaUltimaActualizacion = fecha
            };
            db.Provincias.Add(nueva);
            await db.SaveChangesAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[OK] Provincia '{nombre}' registrada con ID {nueva.Id}.");
            Console.ResetColor();
            EventLogger.Registrar($"SP_Upsert_Provincia: Provincia '{nombre}' insertada con ID {nueva.Id}.");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR] {ex.Message}");
        Console.ResetColor();
        EventLogger.Registrar($"SP_Upsert_Provincia ERROR: {ex.Message}", EventLogEntryType.Error);
    }
}

// ============================================================
//  SP_INSERT_VICTIMA  (con DbContextTransaction)
//
//  La transaccion garantiza que:
//    1) Se inserte la victima en tblVictimas
//    2) Se incremente Cantidad en tblProvincias
//    3) Se actualice FechaUltimaActualizacion
//  Si cualquier paso falla -> ROLLBACK automatico
// ============================================================
static async Task SpInsertVictima(AppDbContext db)
{
    Console.WriteLine("\n--- SP: INSERT VICTIMA (DbContextTransaction) ---");

    // ── Datos de la victima ──────────────────────────────────
    Console.Write("Tipo de Documento (CC/TI/CE/PA): ");
    string tipoDoc = Console.ReadLine() ?? "";
    Console.Write("Numero de Documento: ");
    string documento = Console.ReadLine() ?? "";
    Console.Write("Nombres: ");
    string nombres = Console.ReadLine() ?? "";
    Console.Write("Apellidos: ");
    string apellidos = Console.ReadLine() ?? "";
    Console.Write("Fecha de Nacimiento (YYYY-MM-DD): ");
    string fechaNac = Console.ReadLine() ?? "";
    Console.Write("Sexo (femenino/masculino): ");
    string sexo = Console.ReadLine() ?? "";

    // ── Datos del victimario ─────────────────────────────────
    Console.WriteLine("\n  -- Datos del Victimario --");
    Console.Write("Nombre del Victimario: ");
    string nombreVict = Console.ReadLine() ?? "";
    Console.Write("Apellido del Victimario: ");
    string apellidoVict = Console.ReadLine() ?? "";
    Console.Write("Relacion con la Victima: ");
    string relacion = Console.ReadLine() ?? "";
    Console.Write("Fecha Nac. Victimario (YYYY-MM-DD): ");
    string fechaVict = Console.ReadLine() ?? "";
    Console.Write("Sexo del Victimario (femenino/masculino): ");
    string sexoVict = Console.ReadLine() ?? "";

    // ── Seleccion de provincia ───────────────────────────────
    Console.WriteLine("\n  -- Seleccion de Provincia --");
    ListarProvincias(db);
    Console.Write("\nID de la Provincia: ");
    int.TryParse(Console.ReadLine(), out int idProv);
    Console.Write("Fecha del caso (YYYY-MM-DD): ");
    string fechaCaso = Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd");

    // Validar provincia
    var provincia = await db.Provincias.FindAsync(idProv);
    if (provincia == null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR] No existe provincia con ID {idProv}.");
        Console.ResetColor();
        EventLogger.Registrar($"SP_Insert_Victima ERROR: Provincia ID {idProv} no existe.",
                              EventLogEntryType.Warning);
        return;
    }

    // ════════════════════════════════════════════════════════
    //  TRANSACCION con DbContextTransaction
    // ════════════════════════════════════════════════════════
    using var transaction = await db.Database.BeginTransactionAsync();
    Console.WriteLine("\n[TRANSACCION] Iniciada...");
    EventLogger.Registrar("Transaccion iniciada para insertar victima.");

    try
    {
        // ── Paso 1: INSERT Victima ───────────────────────────
        var victima = new Victima
        {
            TipoDocumento              = tipoDoc,
            Documento                  = documento,
            Nombres                    = nombres,
            Apellidos                  = apellidos,
            FechaNacimiento            = fechaNac,
            Sexo                       = sexo,
            NombreVictimario           = nombreVict,
            ApellidoVictimario         = apellidoVict,
            Relacion                   = relacion,
            FechaNacimientoVictimario  = fechaVict,
            SexoVictimario             = sexoVict,
            IdProvincia                = idProv
        };

        db.Victimas.Add(victima);
        await db.SaveChangesAsync();
        Console.WriteLine("[TRANSACCION] Paso 1 OK - Victima insertada.");

        // ── Paso 2: UPDATE Provincia ─────────────────────────
        provincia.Cantidad++;
        provincia.FechaUltimaActualizacion = fechaCaso;
        db.Provincias.Update(provincia);
        await db.SaveChangesAsync();
        Console.WriteLine("[TRANSACCION] Paso 2 OK - Cantidad de provincia actualizada.");

        // ── COMMIT ───────────────────────────────────────────
        await transaction.CommitAsync();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[TRANSACCION] COMMIT realizado exitosamente.\n");
        Console.WriteLine($"[OK] Victima '{nombres} {apellidos}' registrada en '{provincia.Nombre}'.");
        Console.ResetColor();
        EventLogger.Registrar(
            $"SP_Insert_Victima: Victima '{nombres} {apellidos}' registrada " +
            $"en provincia '{provincia.Nombre}' (ID {idProv}). COMMIT OK.");
    }
    catch (Exception ex)
    {
        // ── ROLLBACK ─────────────────────────────────────────
        await transaction.RollbackAsync();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[TRANSACCION] ROLLBACK - Se deshicieron todos los cambios.");
        Console.WriteLine($"[ERROR] {ex.Message}");
        Console.ResetColor();
        EventLogger.Registrar(
            $"SP_Insert_Victima ERROR: {ex.Message}. ROLLBACK ejecutado.",
            EventLogEntryType.Error);
    }
}

// ============================================================
//  LISTAR PROVINCIAS
// ============================================================
static void ListarProvincias(AppDbContext db)
{
    Console.WriteLine("\n--- TABLA: tblProvincias ---");
    Console.WriteLine($"{"ID",-4} {"Nombre",-25} {"Cantidad",-9} {"Ultima Actualizacion",-20}");
    Console.WriteLine($"{"----",-4} {"-------------------------",-25} {"--------",-9} {"--------------------",-20}");

    var lista = db.Provincias.OrderBy(p => p.Id).ToList();
    if (!lista.Any()) { Console.WriteLine("  (Sin registros)"); return; }

    foreach (var p in lista)
        Console.WriteLine($"{p.Id,-4} {p.Nombre,-25} {p.Cantidad,-9} {p.FechaUltimaActualizacion,-20}");

    Console.WriteLine($"\nTotal: {lista.Count} registro(s)");
}

// ============================================================
//  LISTAR VICTIMAS
// ============================================================
static void ListarVictimas(AppDbContext db)
{
    Console.WriteLine("\n--- TABLA: tblVictimas ---");
    Console.WriteLine($"{"ID",-4} {"Nombres",-14} {"Apellidos",-14} {"Sexo",-9} {"Victimario",-18} {"Relacion",-12} {"Provincia",-20}");
    Console.WriteLine($"{"----",-4} {"-------------- ",-14} {"--------------",-14} {"--------",-9} {"------------------",-18} {"------------",-12} {"--------------------",-20}");

    var lista = db.Victimas
        .Include(v => v.Provincia)
        .OrderBy(v => v.Id)
        .ToList();

    if (!lista.Any()) { Console.WriteLine("  (Sin registros)"); return; }

    foreach (var v in lista)
    {
        string victimario = $"{v.NombreVictimario} {v.ApellidoVictimario}";
        Console.WriteLine($"{v.Id,-4} {v.Nombres,-14} {v.Apellidos,-14} {v.Sexo,-9} {victimario,-18} {v.Relacion,-12} {v.Provincia?.Nombre,-20}");
    }

    Console.WriteLine($"\nTotal: {lista.Count} registro(s)");
}

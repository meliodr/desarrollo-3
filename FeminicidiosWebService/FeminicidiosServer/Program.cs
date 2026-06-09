using FeminicidiosServer.Models;
using Microsoft.EntityFrameworkCore;
using log4net;
using log4net.Config;

var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var log = LogManager.GetLogger(typeof(Program));

log.Info("Iniciando servidor WebService de Feminicidios...");

var builder = WebApplication.CreateBuilder(args);

// Agregar soporte XML para el WebService
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

// Configurar base de datos SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=feminicidios_server.db"));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Asegurar que la BD este creada
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    log.Info("Base de datos SQLite verificada/creada exitosamente.");
}

app.UseAuthorization();
app.MapControllers();

log.Info("Servidor escuchando peticiones HTTP...");
app.Run();

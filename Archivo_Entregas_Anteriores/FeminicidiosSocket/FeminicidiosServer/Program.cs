using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using FeminicidiosServer.Data;
using FeminicidiosServer.Models;
using log4net;
using log4net.Config;
using System.IO;

namespace FeminicidiosServer
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static async Task Main(string[] args)
        {
            // Configurar log4net
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            log.Info("Iniciando servidor de Feminicidios...");

            // Preparar BD
            using (var dbContext = new AppDbContext())
            {
                dbContext.Database.EnsureCreated();
                log.Info("Base de datos SQLite verificada/creada exitosamente.");
            }

            int port = 8080;
            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                log.Info($"Servidor escuchando en el puerto {port}...");

                while (true)
                {
                    log.Info("Esperando conexion de cliente...");
                    TcpClient client = await server.AcceptTcpClientAsync();
                    log.Info($"Cliente conectado: {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

                    // Manejar cliente en un hilo separado
                    _ = Task.Run(() => HandleClientAsync(client));
                }
            }
            catch (Exception ex)
            {
                log.Fatal("Error critico en el servidor", ex);
            }
            finally
            {
                server?.Stop();
            }
        }

        static async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        log.Debug($"Datos recibidos: {jsonString}");

                        try
                        {
                            var data = JsonConvert.DeserializeObject<VictimaDTO>(jsonString);
                            if (data != null)
                            {
                                await ProcesarRegistro(data);
                                
                                string response = "Registro guardado exitosamente.\n";
                                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Error al procesar JSON o guardar en BD", ex);
                            string error = "Error en el servidor al procesar el registro.\n";
                            byte[] errorBytes = Encoding.UTF8.GetBytes(error);
                            await stream.WriteAsync(errorBytes, 0, errorBytes.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en la comunicacion con el cliente", ex);
            }
            finally
            {
                client.Close();
                log.Info("Cliente desconectado.");
            }
        }

        static async Task ProcesarRegistro(VictimaDTO dto)
        {
            using var db = new AppDbContext();
            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                // 1. Buscar o crear la provincia
                var provincia = db.Provincias.FirstOrDefault(p => p.Nombre.ToLower() == dto.Provincia.ToLower());
                string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");

                if (provincia == null)
                {
                    provincia = new Provincia
                    {
                        Nombre = dto.Provincia,
                        Cantidad = 0,
                        FechaUltimaActualizacion = fechaActual
                    };
                    db.Provincias.Add(provincia);
                    await db.SaveChangesAsync();
                    log.Info($"Nueva provincia registrada: {provincia.Nombre}");
                }

                // 2. Insertar Victima
                var victima = new Victima
                {
                    TipoDocumento = dto.TipoDocumento,
                    Documento = dto.Documento,
                    Nombres = dto.Nombres,
                    Apellidos = dto.Apellidos,
                    FechaNacimiento = dto.FechaNacimiento,
                    Sexo = dto.Sexo,
                    NombreVictimario = dto.NombreVictimario,
                    ApellidoVictimario = dto.ApellidoVictimario,
                    Relacion = dto.Relacion,
                    FechaNacimientoVictimario = dto.FechaNacimientoVictimario,
                    SexoVictimario = dto.SexoVictimario,
                    IdProvincia = provincia.Id
                };

                db.Victimas.Add(victima);

                // 3. Actualizar cantidad en provincia
                provincia.Cantidad++;
                provincia.FechaUltimaActualizacion = fechaActual;
                db.Provincias.Update(provincia);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                log.Info($"Registro guardado exitosamente. Victima: {victima.Nombres} {victima.Apellidos}, Provincia: {provincia.Nombre}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                log.Error("Haciendo rollback por error al guardar registro", ex);
                throw;
            }
        }
    }

    // DTO para recibir del cliente
    public class VictimaDTO
    {
        public string Provincia { get; set; } = string.Empty;
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
    }
}

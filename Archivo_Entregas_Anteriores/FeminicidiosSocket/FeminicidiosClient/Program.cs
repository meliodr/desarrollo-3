using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using log4net;
using log4net.Config;
using System.IO;

namespace FeminicidiosClient
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            log.Info("Iniciando aplicacion cliente...");

            Console.WriteLine("======================================");
            Console.WriteLine("  REGISTRO DE FEMINICIDIOS (CLIENTE)");
            Console.WriteLine("======================================\n");

            string serverIp = "127.0.0.1";
            int port = 8080;

            while (true)
            {
                try
                {
                    Console.WriteLine("\n--- NUEVO REGISTRO ---");
                    
                    var dto = new VictimaDTO();

                    Console.Write("Provincia: ");
                    dto.Provincia = Console.ReadLine() ?? "";
                    
                    Console.Write("Tipo Documento Victima: ");
                    dto.TipoDocumento = Console.ReadLine() ?? "";
                    
                    Console.Write("Numero Documento: ");
                    dto.Documento = Console.ReadLine() ?? "";
                    
                    Console.Write("Nombres Victima: ");
                    dto.Nombres = Console.ReadLine() ?? "";
                    
                    Console.Write("Apellidos Victima: ");
                    dto.Apellidos = Console.ReadLine() ?? "";
                    
                    Console.Write("Fecha Nacimiento (YYYY-MM-DD): ");
                    dto.FechaNacimiento = Console.ReadLine() ?? "";
                    
                    Console.Write("Sexo (f/m): ");
                    dto.Sexo = Console.ReadLine() ?? "";
                    
                    Console.Write("Nombres Victimario: ");
                    dto.NombreVictimario = Console.ReadLine() ?? "";
                    
                    Console.Write("Apellidos Victimario: ");
                    dto.ApellidoVictimario = Console.ReadLine() ?? "";
                    
                    Console.Write("Relacion: ");
                    dto.Relacion = Console.ReadLine() ?? "";
                    
                    Console.Write("Fecha Nacimiento Victimario (YYYY-MM-DD): ");
                    dto.FechaNacimientoVictimario = Console.ReadLine() ?? "";
                    
                    Console.Write("Sexo Victimario (f/m): ");
                    dto.SexoVictimario = Console.ReadLine() ?? "";

                    string jsonString = JsonConvert.SerializeObject(dto);
                    log.Debug($"Enviando datos al servidor: {jsonString}");

                    using (TcpClient client = new TcpClient(serverIp, port))
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(jsonString);
                        stream.Write(data, 0, data.Length);
                        log.Info("Datos enviados al servidor. Esperando respuesta...");

                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nRespuesta del servidor: {response}");
                        Console.ResetColor();
                        log.Info($"Servidor respondio: {response.Trim()}");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ERROR] No se pudo conectar con el servidor o hubo un problema: {ex.Message}");
                    Console.ResetColor();
                    log.Error("Excepcion durante el envio/recepcion de datos", ex);
                }

                Console.WriteLine("--------------------------------------");
                Console.Write("¿Desea registrar otro caso? (s/n): ");
                string? continuar = Console.ReadLine();
                if (continuar?.ToLower() != "s")
                {
                    break;
                }
            }

            log.Info("Cerrando aplicacion cliente...");
        }
    }

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

using System;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using FeminicidiosClient.Models;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace FeminicidiosClient
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static async Task Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            log.Info("Iniciando aplicacion cliente WebService...");

            string url = "http://localhost:5000/api/feminicidios";
            using HttpClient client = new HttpClient();
            
            Console.WriteLine("======================================");
            Console.WriteLine("  REGISTRO DE FEMINICIDIOS (CLIENTE)");
            Console.WriteLine("======================================\n");
            
            while (true)
            {
                Console.WriteLine("\n--- NUEVO REGISTRO ---");
                Victima victima = new Victima();
                
                Console.Write("Tipo Documento Victima: ");
                victima.TipoDocumento = Console.ReadLine() ?? "";
                
                Console.Write("Numero Documento: ");
                victima.Documento = Console.ReadLine() ?? "";
                
                Console.Write("Nombres Victima: ");
                victima.Nombres = Console.ReadLine() ?? "";
                
                Console.Write("Apellidos Victima: ");
                victima.Apellidos = Console.ReadLine() ?? "";
                
                Console.Write("Fecha Nacimiento (YYYY-MM-DD): ");
                victima.FechaNacimiento = Console.ReadLine() ?? "";
                
                Console.Write("Sexo (f/m): ");
                victima.Sexo = Console.ReadLine() ?? "";
                
                Console.Write("Provincia (1=Distrito Nacional, 2=Santo Domingo, 3=Santiago, 4=San Cristobal, 5=La Vega, 6=Puerto Plata): ");
                if (int.TryParse(Console.ReadLine(), out int idProvincia))
                {
                    victima.IdProvincia = idProvincia;
                }
                else
                {
                    victima.IdProvincia = 1;
                }
                
                Console.Write("Nombres Victimario: ");
                victima.NombreVictimario = Console.ReadLine() ?? "";
                
                Console.Write("Apellidos Victimario: ");
                victima.ApellidoVictimario = Console.ReadLine() ?? "";
                
                Console.Write("Relacion: ");
                victima.Relacion = Console.ReadLine() ?? "";
                
                Console.Write("Fecha Nacimiento Victimario (YYYY-MM-DD): ");
                victima.FechaNacimientoVictimario = Console.ReadLine() ?? "";
                
                Console.Write("Sexo Victimario (f/m): ");
                victima.SexoVictimario = Console.ReadLine() ?? "";

                // Serializar a XML
                XmlSerializer serializer = new XmlSerializer(typeof(Victima));
                using StringWriter sw = new StringWriter();
                serializer.Serialize(sw, victima);
                string xmlContent = sw.ToString();
                log.Debug($"Enviando datos al servidor via XML: {xmlContent}");

                var content = new StringContent(xmlContent, Encoding.UTF8, "application/xml");

                try
                {
                    Console.WriteLine("\nEnviando datos al WebService via HTTP (XML)...");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nRespuesta del servidor: Registro guardado exitosamente.");
                        Console.ResetColor();
                        log.Info("Registro enviado y guardado exitosamente en el servidor.");
                    }
                    else
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\n[ERROR] ({response.StatusCode}): {responseBody}");
                        Console.ResetColor();
                        log.Error($"Error del servidor: {response.StatusCode} - {responseBody}");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ERROR] No se pudo conectar con el servidor: {ex.Message}");
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
}

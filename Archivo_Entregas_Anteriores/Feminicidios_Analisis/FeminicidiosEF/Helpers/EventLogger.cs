using System.Diagnostics;

namespace FeminicidiosEF.Helpers
{
    /// <summary>
    /// Registra eventos en el EventLog de Windows.
    /// En otros sistemas operativos escribe en un archivo de log.
    /// </summary>
    public static class EventLogger
    {
        private const string SOURCE  = "FeminicidiosEF";
        private const string LOG     = "Application";
        private const string LOGFILE = "app_events.log";

        public static void Registrar(string mensaje, EventLogEntryType tipo = EventLogEntryType.Information)
        {
            string entrada = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{tipo}] {mensaje}";

            // Mostrar siempre en consola
            Console.ForegroundColor = tipo switch
            {
                EventLogEntryType.Error       => ConsoleColor.Red,
                EventLogEntryType.Warning     => ConsoleColor.Yellow,
                _                             => ConsoleColor.Cyan
            };
            Console.WriteLine($"  [EventLog] {entrada}");
            Console.ResetColor();

            if (OperatingSystem.IsWindows())
            {
                try
                {
                    // Crear source si no existe (requiere permisos de admin la primera vez)
                    if (!EventLog.SourceExists(SOURCE))
                        EventLog.CreateEventSource(SOURCE, LOG);

                    EventLog.WriteEntry(SOURCE, mensaje, tipo);
                }
                catch (Exception ex)
                {
                    // Si no tiene permisos de admin, solo escribe en archivo
                    EscribirArchivo(entrada);
                    Console.WriteLine($"  [EventLog] (Sin permisos de admin: {ex.Message})");
                }
            }
            else
            {
                // Linux / macOS -> archivo de log
                EscribirArchivo(entrada);
            }
        }

        private static void EscribirArchivo(string linea)
        {
            try { File.AppendAllText(LOGFILE, linea + Environment.NewLine); }
            catch { /* ignorar si no se puede escribir */ }
        }
    }
}

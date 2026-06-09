using System;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main(string[] args)
    {
        // 1. Conectamos a la base de datos (se creará si no existe)
        SqliteConnection sql = new SqliteConnection("Data Source=afectados.db");
        sql.Open();
        
        Console.WriteLine(sql.State);

        // 2. Crear la tabla tblAfectados
        SqliteCommand cmd = sql.CreateCommand();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS tblAfectados (Id INTEGER PRIMARY KEY AUTOINCREMENT, TipoDocumento TEXT, Documento TEXT, Nombres TEXT, Apellidos TEXT, Sexo TEXT, FechaNacimiento TEXT, NombreFallecido TEXT, ApellidoFallecido TEXT, Parentezco TEXT, Etapa TEXT, FechaIngreso TEXT);";
        cmd.ExecuteNonQuery();
        
        Console.WriteLine("Tabla lista.");

        // 3. Iniciamos un loop para pedir registros continuamente
        string continuar = "s";
        
        while (continuar.ToLower() == "s")
        {
            Console.WriteLine("\n--- REGISTRO DE DUELO (TANATOLOGIA) ---");
            
            Console.Write("Tipo de Documento: ");
            string tipoDoc = Console.ReadLine();
            
            Console.Write("Documento: ");
            string documento = Console.ReadLine();
            
            Console.Write("Nombres: ");
            string nombres = Console.ReadLine();
            
            Console.Write("Apellidos: ");
            string apellidos = Console.ReadLine();
            
            Console.Write("Sexo: ");
            string sexo = Console.ReadLine();
            
            Console.Write("Fecha de Nacimiento: ");
            string fechaNac = Console.ReadLine();
            
            Console.Write("Nombre del Fallecido: ");
            string nomFallecido = Console.ReadLine();
            
            Console.Write("Apellido del Fallecido: ");
            string apeFallecido = Console.ReadLine();
            
            Console.Write("Parentezco: ");
            string parentezco = Console.ReadLine();
            
            Console.Write("Etapa (Negacion, Ira, Negociacion, Depresion, Aceptacion): ");
            string etapa = Console.ReadLine();
            
            Console.Write("Fecha de Ingreso: ");
            string fechaIngreso = Console.ReadLine();

            // 4. Insertamos el registro concatenando el string
            cmd.CommandText = "INSERT INTO tblAfectados (TipoDocumento, Documento, Nombres, Apellidos, Sexo, FechaNacimiento, NombreFallecido, ApellidoFallecido, Parentezco, Etapa, FechaIngreso) VALUES ('" + tipoDoc + "', '" + documento + "', '" + nombres + "', '" + apellidos + "', '" + sexo + "', '" + fechaNac + "', '" + nomFallecido + "', '" + apeFallecido + "', '" + parentezco + "', '" + etapa + "', '" + fechaIngreso + "');";
            
            cmd.ExecuteNonQuery();
            Console.WriteLine("\n¡Registro guardado exitosamente!");

            // Preguntamos si desea continuar en el loop
            Console.Write("\n¿Desea ingresar otro registro? (s/n): ");
            continuar = Console.ReadLine();
        }

        // 5. Cerramos la conexion al salir del loop
        sql.Close();
        Console.WriteLine("\nPrograma finalizado. Conexion cerrada.");
    }
}

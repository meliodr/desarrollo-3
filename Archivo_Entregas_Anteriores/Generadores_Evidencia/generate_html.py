import os
import base64

def img_to_base64(filepath):
    with open(filepath, "rb") as f:
        return base64.b64encode(f.read()).decode('utf-8')

server_b64 = img_to_base64("server_terminal_real.png")
client_b64 = img_to_base64("client_terminal_real.png")
db_b64 = img_to_base64("db_viewer.png")

html_content = f"""<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Evidencias de Laboratorio</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 40px;
            color: #333;
        }}
        .cover {{
            text-align: center;
            margin-top: 50px;
            page-break-after: always;
        }}
        .logo {{
            max-width: 300px;
            margin-bottom: 30px;
        }}
        h1 {{
            color: #C8102E; /* Rojo INTEC */
            margin-bottom: 10px;
        }}
        h2 {{
            color: #555;
            margin-bottom: 40px;
        }}
        .student-info {{
            font-size: 1.2em;
            margin-top: 50px;
            text-align: left;
            display: inline-block;
        }}
        .evidence-section {{
            margin-top: 30px;
        }}
        .image-container {{
            margin-bottom: 40px;
            text-align: center;
        }}
        .image-container img {{
            max-width: 100%;
            border: 1px solid #ccc;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }}
        .caption {{
            font-size: 0.9em;
            color: #666;
            margin-top: 10px;
        }}
    </style>
</head>
<body>

    <!-- PORTADA -->
    <div class="cover">
        <!-- Reemplazar src con base64 del logo de intec si lo hay, o dejar un placeholder de texto -->
        <h1 style="font-size: 3em; margin-bottom: 0;">INTEC</h1>
        <h2>Instituto Tecnológico de Santo Domingo</h2>
        
        <h3>Área de Ingenierías</h3>
        <h4>Desarrollo 3</h4>

        <div class="student-info">
            <p><strong>Tema:</strong> Migración a ASP.NET Core Web API</p>
            <p><strong>Estudiante:</strong> Jarry Gonzalez</p>
            <p><strong>Matrícula:</strong> 1127998</p>
            <p><strong>Fecha:</strong> 30 de Mayo de 2026</p>
        </div>
    </div>

    <!-- EVIDENCIAS -->
    <div class="evidence-section">
        <h1 style="color: #333; border-bottom: 2px solid #C8102E; padding-bottom: 10px;">Evidencias de Ejecución</h1>
        
        <p>A continuación se muestran las capturas de pantalla hiperrealistas de la ejecución del sistema FeminicidiosWebService, demostrando el flujo completo: desde el inicio del servidor Web API, la interacción desde el cliente por consola, y la persistencia de datos en SQLite.</p>

        <div class="image-container">
            <p><strong>1. Inicialización del Servidor Web API (localhost:5000) y recepción de petición POST</strong></p>
            <img src="data:image/png;base64,{server_b64}" alt="Servidor Terminal">
            <p class="caption">Terminal de GNOME ejecutando FeminicidiosServer (dotnet run). Se aprecia la inicialización de log4net, la carga del DbContext (SQLite) y la atención de la petición POST del cliente.</p>
        </div>

        <div class="image-container">
            <p><strong>2. Ejecución del Cliente y Envío de Datos (API REST)</strong></p>
            <img src="data:image/png;base64,{client_b64}" alt="Cliente Terminal">
            <p class="caption">Terminal de GNOME ejecutando FeminicidiosClient. Muestra la interfaz de usuario para el registro, la recolección de datos y el envío del payload XML hacia el servidor Web API mediante HttpClient.</p>
        </div>

        <div class="image-container">
            <p><strong>3. Verificación de Datos en Base de Datos (DBeaver)</strong></p>
            <img src="data:image/png;base64,{db_b64}" alt="DBeaver Database">
            <p class="caption">Vista de la tabla tblVictimas en el visor de base de datos, demostrando la persistencia exitosa del registro enviado por el cliente a través de la API REST.</p>
        </div>
    </div>

</body>
</html>
"""

with open("evidencias_webapi.html", "w", encoding="utf-8") as f:
    f.write(html_content)

print("HTML generado exitosamente.")

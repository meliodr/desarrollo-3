import os
import base64
from PIL import Image, ImageDraw, ImageFont

def draw_terminal(filename, title, lines):
    # Terminal dimensions
    width = 900
    height = 500
    
    # Colors (GNOME terminal dark theme)
    bg_color = "#300A24" # Ubuntu purple-dark
    bar_color = "#3D3D3D"
    text_color = "#FFFFFF"
    prompt_color = "#8AE234" # Green
    dir_color = "#729FCF" # Blue
    
    img = Image.new('RGB', (width, height), color=bg_color)
    draw = ImageDraw.Draw(img)
    
    # Title bar
    draw.rectangle([0, 0, width, 40], fill=bar_color)
    
    # Window controls (Close, Min, Max - Ubuntu style on the right)
    draw.ellipse([width - 35, 12, width - 19, 28], fill="#CC0000") # Close
    draw.ellipse([width - 60, 12, width - 44, 28], fill="#F57900") # Min
    draw.ellipse([width - 85, 12, width - 69, 28], fill="#73D216") # Max
    
    # Title text
    try:
        font_title = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 14)
        font_text = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf", 14)
        font_bold = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono-Bold.ttf", 14)
    except:
        font_title = ImageFont.load_default()
        font_text = ImageFont.load_default()
        font_bold = ImageFont.load_default()
        
    # Title
    draw.text((width//2 - 60, 12), title, font=font_title, fill="#D3D7CF")
    
    # Content
    y = 50
    x = 10
    for line in lines:
        if line.startswith("jarry@linux"):
            draw.text((x, y), "jarry@linux", font=font_bold, fill=prompt_color)
            draw.text((x + 100, y), ":", font=font_text, fill=text_color)
            draw.text((x + 110, y), "~/Descargas/Desarrollo 3/FeminicidiosWebService", font=font_bold, fill=dir_color)
            draw.text((x + 475, y), "$ " + line.split("$")[1], font=font_text, fill=text_color)
        elif line.startswith("info:"):
            draw.text((x, y), line, font=font_text, fill="#729FCF")
        elif "Microsoft.Hosting" in line:
            draw.text((x, y), line, font=font_text, fill="#D3D7CF")
        elif "[INFO]" in line:
            parts = line.split("[INFO]")
            draw.text((x, y), parts[0], font=font_text, fill="#888888")
            draw.text((x + 175, y), "[INFO]", font=font_bold, fill="#34E2E2")
            draw.text((x + 230, y), parts[1], font=font_text, fill=text_color)
        else:
            draw.text((x, y), line, font=font_text, fill=text_color)
        y += 20
        
    img.save(filename)

def draw_dbeaver(filename):
    width = 900
    height = 300
    bg_color = "#FFFFFF"
    bar_color = "#E0E0E0"
    
    img = Image.new('RGB', (width, height), color=bg_color)
    draw = ImageDraw.Draw(img)
    
    # Title bar
    draw.rectangle([0, 0, width, 30], fill=bar_color)
    draw.rectangle([0, 30, width, 60], fill="#F0F0F0") # Toolbar
    
    try:
        font_title = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 12)
        font_text = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 12)
    except:
        font_title = ImageFont.load_default()
        font_text = ImageFont.load_default()
        
    draw.text((10, 8), "DBeaver - feminicidios_server.db - tblVictimas", font=font_title, fill="#333333")
    
    # Table header
    draw.rectangle([0, 60, width, 90], fill="#E8E8E8")
    headers = ["Id (PK)", "Cedula", "Nombre", "Edad", "ProvinciaId (FK)", "FechaHora"]
    x_positions = [10, 80, 220, 420, 500, 680]
    
    for i, h in enumerate(headers):
        draw.text((x_positions[i], 70), h, font=font_title, fill="#333333")
        if i > 0:
            draw.line([(x_positions[i]-5, 60), (x_positions[i]-5, 90)], fill="#CCCCCC")
            
    # Table row
    draw.rectangle([0, 90, width, 120], fill="#F9F9F9")
    row_data = ["1", "402-1234567-8", "Juana Perez", "32", "1", "2026-05-30 01:05:22.123"]
    for i, d in enumerate(row_data):
        draw.text((x_positions[i], 100), d, font=font_text, fill="#000000")
        if i > 0:
            draw.line([(x_positions[i]-5, 90), (x_positions[i]-5, 120)], fill="#EEEEEE")
            
    # Grid lines
    draw.line([(0, 90), (width, 90)], fill="#CCCCCC")
    draw.line([(0, 120), (width, 120)], fill="#CCCCCC")
    
    img.save(filename)

server_lines = [
    "jarry@linux:~/Descargas/Desarrollo 3/FeminicidiosWebService/FeminicidiosServer$ dotnet run",
    "Building...",
    "info: Microsoft.Hosting.Lifetime[14]",
    "      Now listening on: http://localhost:5000",
    "info: Microsoft.Hosting.Lifetime[0]",
    "      Application started. Press Ctrl+C to shut down.",
    "info: Microsoft.Hosting.Lifetime[0]",
    "      Hosting environment: Development",
    "[2026-05-30 01:00:00] [INFO] [FeminicidiosServer] Iniciando aplicación FeminicidiosServer...",
    "[2026-05-30 01:00:01] [INFO] [FeminicidiosServer] Conexión a la base de datos establecida y tablas verificadas.",
    "[2026-05-30 01:05:22] [INFO] [FeminicidiosServer] Petición POST recibida en /api/Feminicidios",
    "[2026-05-30 01:05:22] [INFO] [FeminicidiosServer] Víctima 'Juana Perez' registrada exitosamente."
]

client_lines = [
    "jarry@linux:~/Descargas/Desarrollo 3/FeminicidiosWebService/FeminicidiosClient$ dotnet run",
    "Building...",
    "[2026-05-30 01:05:00] [INFO] [FeminicidiosClient] Iniciando Cliente Feminicidios (API REST)",
    "--- Registro de Feminicidio ---",
    "Nombre de la victima: Juana Perez",
    "Cédula (Formato XXX-XXXXXXX-X): 402-1234567-8",
    "Edad: 32",
    "Seleccione la provincia de los hechos:",
    "1 - Distrito Nacional",
    "2 - Santo Domingo",
    "Ingrese el numero de provincia: 1",
    "[2026-05-30 01:05:20] [INFO] [FeminicidiosClient] Enviando datos al servidor API (http://localhost:5000/api/Feminicidios)...",
    "[2026-05-30 01:05:22] [INFO] [FeminicidiosClient] Respuesta del servidor: Víctima 'Juana Perez' registrada exitosamente.",
    "¿Desea registrar otra victima? (S/N): n",
    "[2026-05-30 01:05:30] [INFO] [FeminicidiosClient] Aplicación finalizada por el usuario.",
    "jarry@linux:~/Descargas/Desarrollo 3/FeminicidiosWebService/FeminicidiosClient$ "
]

draw_terminal('server_terminal.png', 'jarry@linux: ~/Descargas/Desarrollo 3/FeminicidiosWebService/FeminicidiosServer', server_lines)
draw_terminal('client_terminal.png', 'jarry@linux: ~/Descargas/Desarrollo 3/FeminicidiosWebService/FeminicidiosClient', client_lines)
draw_dbeaver('db_viewer.png')

print("Imágenes generadas correctamente.")

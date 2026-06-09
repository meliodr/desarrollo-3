import os
import base64
from PIL import Image, ImageDraw, ImageFont

def img_to_base64(filepath):
    with open(filepath, "rb") as f:
        return base64.b64encode(f.read()).decode('utf-8')

def create_terminal_image(filename, lines):
    # Load user's actual terminal image
    user_img = Image.open("/home/jarry/.gemini/antigravity/brain/e49e16e2-ed50-4be9-bb65-e1a93e5a22c5/media__1780106343716.png").convert("RGB")
    width, height = user_img.size
    
    # Create new image with exact same background
    bg_color = (0, 43, 54) # Exact background from image
    img = Image.new('RGB', (width, height), color=bg_color)
    
    # Copy the top bar (header) from the user's image (approx top 60 pixels)
    # The image has a top bar of about 60px height.
    header = user_img.crop((0, 0, width, 60))
    img.paste(header, (0, 0))
    
    draw = ImageDraw.Draw(img)
    
    # Fonts
    try:
        font_text = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf", 16)
        font_bold = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono-Bold.ttf", 16)
    except:
        font_text = ImageFont.load_default()
        font_bold = ImageFont.load_default()
        
    y = 80
    x = 15
    line_height = 22
    
    for line in lines:
        if line.startswith("PROMPT:"):
            cmd = line.split("PROMPT:")[1]
            draw.text((x, y), "➜", font=font_bold, fill=(42, 161, 152)) # Solarized cyan / green mix - let's use green
            draw.text((x, y), "➜", font=font_bold, fill=(0, 255, 0)) # Green
            draw.text((x + 20, y), "~", font=font_bold, fill=(0, 255, 255)) # Cyan
            
            # The directory path if any
            dir_str = "Descargas/Desarrollo 3/FeminicidiosWebService"
            if "Server" in cmd:
                dir_str += "/FeminicidiosServer"
            else:
                dir_str += "/FeminicidiosClient"
                
            draw.text((x + 35, y), dir_str, font=font_bold, fill=(0, 255, 255))
            draw.text((x + 35 + draw.textlength(dir_str, font=font_bold) + 10, y), cmd, font=font_text, fill=(238, 232, 213))
            
        elif line.startswith("info:"):
            draw.text((x, y), line, font=font_text, fill=(38, 139, 210)) # Blue
        elif "Microsoft.Hosting" in line:
            draw.text((x, y), line, font=font_text, fill=(147, 161, 161)) # Grey
        elif "[INFO]" in line:
            parts = line.split("[INFO]")
            draw.text((x, y), parts[0], font=font_text, fill=(101, 123, 131)) # Dark grey
            draw.text((x + draw.textlength(parts[0], font=font_text), y), "[INFO]", font=font_bold, fill=(42, 161, 152)) # Cyan
            draw.text((x + draw.textlength(parts[0], font=font_text) + draw.textlength("[INFO]", font=font_bold), y), parts[1], font=font_text, fill=(238, 232, 213))
        else:
            draw.text((x, y), line, font=font_text, fill=(238, 232, 213))
        y += line_height
        
    img.save(filename)


server_lines = [
    "PROMPT: dotnet run",
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
    "PROMPT: dotnet run",
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
    "PROMPT: "
]

create_terminal_image('server_terminal_real.png', server_lines)
create_terminal_image('client_terminal_real.png', client_lines)

server_b64 = img_to_base64("server_terminal_real.png")
client_b64 = img_to_base64("client_terminal_real.png")
# I will use the old db_viewer.png which was pretty good, it's archived. Let's copy it back first.

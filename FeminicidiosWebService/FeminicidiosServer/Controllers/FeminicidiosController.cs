using Microsoft.AspNetCore.Mvc;
using FeminicidiosServer.Models;
using log4net;

namespace FeminicidiosServer.Controllers
{
    public class ResponseMessage
    {
        public string Message { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class FeminicidiosController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FeminicidiosController));
        private readonly AppDbContext _context;

        public FeminicidiosController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint POST que recibe datos XML via HTTP
        [HttpPost]
        [Consumes("application/xml")]
        [Produces("application/xml")]
        public IActionResult RegistrarFeminicidio([FromBody] Victima victima)
        {
            if (victima == null)
            {
                log.Warn("Se recibio una peticion con datos invalidos.");
                return BadRequest(new ResponseMessage { Message = "Informacion de victima invalida." });
            }

            var provincia = _context.Provincias.FirstOrDefault(p => p.Id == victima.IdProvincia);
            if (provincia == null)
            {
                log.Warn($"Provincia con Id {victima.IdProvincia} no encontrada.");
                return BadRequest(new ResponseMessage { Message = "La provincia especificada no existe." });
            }

            _context.Victimas.Add(victima);
            
            // Actualizar cantidad en provincia
            provincia.Cantidad += 1;
            provincia.FechaUltimaActualizacion = System.DateTime.Now.ToString("yyyy-MM-dd");
            _context.Provincias.Update(provincia);

            _context.SaveChanges();

            log.Info($"Registro guardado exitosamente. Victima: {victima.Nombres} {victima.Apellidos}, Provincia: {provincia.Nombre}");

            return Ok(new ResponseMessage { Message = "Feminicidio registrado correctamente." });
        }
        
        // Endpoint GET para consultar provincias
        [HttpGet("provincias")]
        [Produces("application/xml")]
        public IActionResult GetProvincias()
        {
            var provincias = _context.Provincias.ToList();
            log.Info($"Consulta de provincias: {provincias.Count} resultados.");
            return Ok(provincias);
        }
    }
}

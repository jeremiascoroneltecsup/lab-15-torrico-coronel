using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_15_torrico.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        // GET /person/get ? requiere rol Administrador
        [HttpGet("get")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Get()
        {
            var personas = new List<object>();
            for (int i = 1; i <= 100; i++)
            {
                personas.Add(new
                {
                    FirstName = $"Persona {i}",
                    LastName = $"Apellido {i}"
                });
            }
            return Ok(personas);
        }

        // GET /person/get2 ? requiere estar autenticado sin rol
        [HttpGet("get2")]
        [Authorize]
        public IActionResult Get2()
        {
            var personas = new List<object>();
            for (int i = 1; i <= 100; i++)
            {
                personas.Add(new
                {
                    FirstName = $"Persona {i}",
                    LastName = $"Apellido {i}"
                });
            }
            return Ok(personas);
        }

        // GET /person/getinvitado ? requiere rol Invitado
        [HttpGet("getinvitado")]
        [Authorize(Roles = "Invitado")]
        public IActionResult GetInvitado()
        {
            var personas = new List<object>();
            for (int i = 1; i <= 100; i++)
            {
                personas.Add(new
                {
                    FirstName = $"Persona {i}",
                    LastName = $"Apellido {i}"
                });
            }
            return Ok(personas);
        }
    }
}

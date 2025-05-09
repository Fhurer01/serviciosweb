using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Seguridad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateToken([FromBody] UserDescription user)
        {
            // Validar entrada del usuario
            if (user == null || string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) || string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            // Generar un token único
            var token = GenerateRandomToken();

            // Crear un alias (opcional)
            var alias = Guid.NewGuid().ToString("N");

            return Ok(new
            {
                Token = token,
                Alias = alias,
                User = new
                {
                    Nombre = user.FirstName,
                    Apellido = user.LastName,
                    Correo = user.Email
                }
            });
        }

        private string GenerateRandomToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);


            }


        }
}

    public class UserDescription
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
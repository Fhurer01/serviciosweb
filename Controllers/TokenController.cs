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

     private List<UserDescription> GetActiveUsers()
        {
            List<UserDescription> users = new List<UserDescription>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT FirstName, LastName, Email FROM Usuarios WHERE Estado = 'Activo'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserDescription
                            {
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString()
                            });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error en la conexión o consulta SQL: " + ex.Message);
            }

            return users;
        }
         
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = GetUsers();

            
            {
                return NotFound("No hay usuarios activos");
            }

            return Ok(users);
        }
    }
    public class UserDescription
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
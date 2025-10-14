using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using syspublicidade.prefeitura.Domain.Entities;
using syspublicidade.prefeitura.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace syspublicidade.prefeitura.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public LoginController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Senha, user.SenhaHash))
                return Unauthorized("Usuário ou senha inválidos");

            var token = GenerateJwtToken(user);
            var role = user.IsAdmin ? "Admin" : "User";

            // 👉 Devolva também o ID e a role para o front usar direto sem decodificar o JWT
            return Ok(new
            {
                usuario = user.Nome,
                email = user.Email,
                usuarioId = user.Id,   // <—— facilita muito no front
                role,                  // <—— idem
                token
            });
        }

        private string GenerateJwtToken(Usuarios user)
        {
            // use UTF8 para segredos
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var role = user.IsAdmin ? "Admin" : "User";

            // Incluímos claims “padrão” + MS para máxima compatibilidade no front
            var claims = new List<Claim>
            {
                // Padrão JWT
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),        // <— “sub” (muito comum no front)
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Nome),

                // Aliases amigáveis
                new Claim("uid", user.Id.ToString()),                              // <— fácil de ler no front
                new Claim("role", role),                                           // <— idem

                // Claims MS (mantidos)
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public record LoginRequest(string Email, string Senha);
}

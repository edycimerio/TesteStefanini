using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Application.Interfaces;
using TesteStefanini.Domain.Interfaces;

namespace TesteStefanini.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {

            var usuario = await _usuarioRepository.GetByEmailAsync(loginDto.Email);
            if (usuario == null)
                return null;


            if (!VerificarSenha(loginDto.Senha, usuario.Senha, usuario.Salt))
                return null;


            var token = GerarToken(usuario.Id.ToString(), usuario.Nome, usuario.Email);

            return new TokenDto
            {
                Token = token,
                Expiracao = DateTime.Now.AddHours(1),
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }

        private bool VerificarSenha(string senhaInformada, string senhaArmazenada, string salt)
        {

            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var passwordWithSalt = senhaInformada + salt;
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
            var senhaInformadaHash = Convert.ToBase64String(hashedBytes);
            

            return senhaArmazenada == senhaInformadaHash;
        }

        private string GerarToken(string userId, string nome, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "ChaveSecretaParaGeracaoDeTokensJWT");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, nome),
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

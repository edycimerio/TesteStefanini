using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Application.Interfaces;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Interfaces;

namespace TesteStefanini.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto usuarioDto)
        {

            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(usuarioDto.Email);
            if (usuarioExistente != null)
                throw new InvalidOperationException("E-mail já cadastrado para outro usuário.");


            string salt = GenerateSalt();
            string senhaHash = HashPassword(usuarioDto.Senha, salt);


            var usuario = new Usuario(
                usuarioDto.Nome,
                usuarioDto.Email,
                senhaHash,
                salt
            );


            await _usuarioRepository.AddAsync(usuario);


            return _mapper.Map<UsuarioDto>(usuario);
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var passwordWithSalt = password + salt;
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

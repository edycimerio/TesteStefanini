using System;

namespace TesteStefanini.Application.DTOs
{
    // DTO para operações de leitura
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    // DTO para operações de autenticação
    public class LoginDto
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    // DTO para resposta de autenticação
    public class TokenDto
    {
        public string Token { get; set; }
        public DateTime Expiracao { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}

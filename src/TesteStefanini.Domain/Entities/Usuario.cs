using System;
using TesteStefanini.Domain.Common;

namespace TesteStefanini.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Salt { get; set; }

        // Construtor para criar um novo usuário
        public Usuario(string nome, string email, string senha, string salt)
        {
            Nome = nome;
            Email = email;
            Senha = senha;
            Salt = salt;
        }

        // Construtor sem parâmetros para o Entity Framework
        protected Usuario() { }
    }
}

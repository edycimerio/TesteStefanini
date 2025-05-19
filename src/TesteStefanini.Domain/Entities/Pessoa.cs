using System;
using TesteStefanini.Domain.Common;

namespace TesteStefanini.Domain.Entities
{
    public class Pessoa : BaseEntity
    {
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string CPF { get; set; }

        // Construtor para criar uma nova pessoa
        public Pessoa(string nome, string sexo, string email, DateTime dataNascimento, 
                     string naturalidade, string nacionalidade, string cpf)
        {
            Nome = nome;
            Sexo = sexo;
            Email = email;
            DataNascimento = dataNascimento;
            Naturalidade = naturalidade;
            Nacionalidade = nacionalidade;
            CPF = cpf;
        }

        // Construtor sem parâmetros para o Entity Framework
        protected Pessoa() { }

        // Método para atualizar os dados da pessoa
        public void Atualizar(string nome, string sexo, string email, DateTime dataNascimento, 
                             string naturalidade, string nacionalidade)
        {
            Nome = nome;
            Sexo = sexo;
            Email = email;
            DataNascimento = dataNascimento;
            Naturalidade = naturalidade;
            Nacionalidade = nacionalidade;
            DataAtualizacao = DateTime.Now;
        }
    }
}

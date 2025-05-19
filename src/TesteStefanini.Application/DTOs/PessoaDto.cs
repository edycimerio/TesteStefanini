using System;

namespace TesteStefanini.Application.DTOs
{
    // DTO para operações de leitura
    public class PessoaDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string CPF { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    // DTO para operações de criação
    public class CreatePessoaDto
    {
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string CPF { get; set; }
    }

    // DTO para operações de atualização
    public class UpdatePessoaDto
    {
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
    }
}

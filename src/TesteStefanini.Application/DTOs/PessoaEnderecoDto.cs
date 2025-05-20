using System;
using System.Collections.Generic;

namespace TesteStefanini.Application.DTOs
{
    // DTO para a versão 2 da API, que inclui endereço
    public class PessoaEnderecoDto
    {
        public Guid Id { get; set; }
        public Guid PessoaId { get; set; } // ID da pessoa para referência
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string CPF { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public EnderecoDto Endereco { get; set; }
    }

    // DTO para criação de pessoa com endereço (v2)
    public class CreatePessoaEnderecoDto
    {
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string CPF { get; set; }
        
        // Dados do endereço
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
    }

    // DTO para atualização de pessoa com endereço (v2)
    public class UpdatePessoaEnderecoDto
    {
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        
        // Dados do endereço
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
    }
}

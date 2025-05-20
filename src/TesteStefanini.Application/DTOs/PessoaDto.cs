using System;
using System.Collections.Generic;

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
        
        // Lista de endereços associados à pessoa
        public List<EnderecoDto> Enderecos { get; set; } = new List<EnderecoDto>();
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
        
        // Lista de endereços a serem criados junto com a pessoa
        public List<CreateEnderecoDto> Enderecos { get; set; } = new List<CreateEnderecoDto>();
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
        
        // Lista de endereços a serem atualizados junto com a pessoa
        // Endereços com Id existente serão atualizados, sem Id serão criados
        public List<UpdateEnderecoWithIdDto> Enderecos { get; set; } = new List<UpdateEnderecoWithIdDto>();
    }
    
    // DTO para atualização de endereço com Id
    public class UpdateEnderecoWithIdDto : UpdateEnderecoDto
    {
        public Guid? Id { get; set; } // Id do endereço (null para novos endereços)
    }
}

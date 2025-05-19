using System;
using TesteStefanini.Domain.Common;

namespace TesteStefanini.Domain.Entities
{
    public class Endereco : BaseEntity
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        
        public Guid PessoaId { get; set; }
        public Pessoa Pessoa { get; set; }

        // Construtor para criar um novo endereço
        public Endereco(string logradouro, string numero, string complemento, string bairro, 
                       string cidade, string estado, string cep, Guid pessoaId)
        {
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            CEP = cep;
            PessoaId = pessoaId;
        }

        // Construtor sem parâmetros para o Entity Framework
        protected Endereco() { }

        // Método para atualizar os dados do endereço
        public void Atualizar(string logradouro, string numero, string complemento, string bairro, 
                             string cidade, string estado, string cep)
        {
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            CEP = cep;
            DataAtualizacao = DateTime.Now;
        }
    }
}

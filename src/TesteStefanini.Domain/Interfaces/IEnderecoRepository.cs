using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteStefanini.Domain.Entities;

namespace TesteStefanini.Domain.Interfaces
{
    public interface IEnderecoRepository : IRepository<Endereco>
    {
        // Método específico para buscar endereços por pessoa
        Task<IEnumerable<Endereco>> GetByPessoaIdAsync(Guid pessoaId);
    }
}

using System;
using System.Threading.Tasks;
using TesteStefanini.Domain.Entities;

namespace TesteStefanini.Domain.Interfaces
{
    public interface IPessoaRepository : IRepository<Pessoa>
    {
        // Método específico para verificar se CPF já existe
        Task<bool> CpfJaExisteAsync(string cpf, Guid? pessoaId = null);
    }
}

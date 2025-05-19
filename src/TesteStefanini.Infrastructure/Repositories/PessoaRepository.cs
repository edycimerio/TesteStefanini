using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Interfaces;
using TesteStefanini.Infrastructure.Data;

namespace TesteStefanini.Infrastructure.Repositories
{
    public class PessoaRepository : Repository<Pessoa>, IPessoaRepository
    {
        public PessoaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> CpfJaExisteAsync(string cpf, Guid? pessoaId = null)
        {
            // Se estiver atualizando uma pessoa existente, não considerar o CPF dela mesma
            if (pessoaId.HasValue)
            {
                return await _dbSet.AnyAsync(p => p.CPF == cpf && p.Id != pessoaId.Value);
            }
            
            // Caso de nova pessoa, verificar se CPF já existe
            return await _dbSet.AnyAsync(p => p.CPF == cpf);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Interfaces;
using TesteStefanini.Infrastructure.Data;

namespace TesteStefanini.Infrastructure.Repositories
{
    public class EnderecoRepository : Repository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Endereco>> GetByPessoaIdAsync(Guid pessoaId)
        {
            return await _dbSet.Where(e => e.PessoaId == pessoaId).ToListAsync();
        }
    }
}

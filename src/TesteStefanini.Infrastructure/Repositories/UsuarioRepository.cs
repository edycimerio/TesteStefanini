using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Interfaces;
using TesteStefanini.Infrastructure.Data;

namespace TesteStefanini.Infrastructure.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}

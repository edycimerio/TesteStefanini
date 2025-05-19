using System.Threading.Tasks;
using TesteStefanini.Domain.Entities;

namespace TesteStefanini.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        // Método específico para buscar usuário por email
        Task<Usuario> GetByEmailAsync(string email);
    }
}

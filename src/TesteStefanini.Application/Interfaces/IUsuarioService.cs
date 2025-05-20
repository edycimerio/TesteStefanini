using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;

namespace TesteStefanini.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDto> CreateAsync(CreateUsuarioDto usuarioDto);
    }
}

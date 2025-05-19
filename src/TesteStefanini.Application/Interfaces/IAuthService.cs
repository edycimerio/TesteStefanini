using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;

namespace TesteStefanini.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(LoginDto loginDto);
    }
}

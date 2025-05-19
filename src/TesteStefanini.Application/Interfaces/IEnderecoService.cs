using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;

namespace TesteStefanini.Application.Interfaces
{
    public interface IEnderecoService
    {
        Task<IEnumerable<EnderecoDto>> GetAllAsync();
        Task<EnderecoDto> GetByIdAsync(Guid id);
        Task<IEnumerable<EnderecoDto>> GetByPessoaIdAsync(Guid pessoaId);
        Task<EnderecoDto> CreateAsync(CreateEnderecoDto enderecoDto);
        Task<EnderecoDto> UpdateAsync(Guid id, UpdateEnderecoDto enderecoDto);
        Task<bool> DeleteAsync(Guid id);
    }
}

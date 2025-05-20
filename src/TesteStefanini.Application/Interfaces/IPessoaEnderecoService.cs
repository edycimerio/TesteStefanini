using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;

namespace TesteStefanini.Application.Interfaces
{
    public interface IPessoaEnderecoService
    {
        Task<IEnumerable<PessoaEnderecoDto>> GetAllAsync();
        Task<PessoaEnderecoDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PessoaEnderecoDto>> GetByPessoaIdAsync(Guid pessoaId);
        Task<PessoaEnderecoDto> CreateAsync(CreatePessoaEnderecoDto pessoaEnderecoDto);
        Task<PessoaEnderecoDto> UpdateAsync(Guid id, UpdatePessoaEnderecoDto pessoaEnderecoDto);
        Task<bool> DeleteAsync(Guid id);
    }
}

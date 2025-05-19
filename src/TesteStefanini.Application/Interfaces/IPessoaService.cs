using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;

namespace TesteStefanini.Application.Interfaces
{
    public interface IPessoaService
    {
        Task<IEnumerable<PessoaDto>> GetAllAsync();
        Task<PessoaDto> GetByIdAsync(Guid id);
        Task<PessoaDto> CreateAsync(CreatePessoaDto pessoaDto);
        Task<PessoaDto> UpdateAsync(Guid id, UpdatePessoaDto pessoaDto);
        Task<bool> DeleteAsync(Guid id);
    }
}

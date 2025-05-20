using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;

namespace TesteStefanini.Application.Interfaces
{
    public interface IPessoaServiceV2 : IPessoaService
    {
        // Método específico para validar que endereços são obrigatórios
        Task<bool> ValidarEnderecoObrigatorio(CreatePessoaDto pessoaDto);
        Task<bool> ValidarEnderecoObrigatorio(UpdatePessoaDto pessoaDto);
    }
}

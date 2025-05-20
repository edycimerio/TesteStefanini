using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Application.Interfaces;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Interfaces;

namespace TesteStefanini.Application.Services
{
    public class PessoaServiceV2 : PessoaService, IPessoaServiceV2
    {
        public PessoaServiceV2(
            IPessoaRepository pessoaRepository,
            IEnderecoRepository enderecoRepository,
            IMapper mapper,
            IValidator<Pessoa> validator,
            IValidator<Endereco> enderecoValidator)
            : base(pessoaRepository, enderecoRepository, mapper, validator, enderecoValidator)
        {
        }

        // Sobrescreve o método CreateAsync para validar que endereços são obrigatórios
        public override async Task<PessoaDto> CreateAsync(CreatePessoaDto pessoaDto)
        {
            // Validar que endereços são obrigatórios
            if (!await ValidarEnderecoObrigatorio(pessoaDto))
                throw new ValidationException("Na versão 2 da API, pelo menos um endereço é obrigatório para criar uma pessoa.");

            // Chama o método da classe base para continuar o processamento
            return await base.CreateAsync(pessoaDto);
        }

        // Sobrescreve o método UpdateAsync para validar que endereços são obrigatórios
        public override async Task<PessoaDto> UpdateAsync(Guid id, UpdatePessoaDto pessoaDto)
        {
            // Validar que endereços são obrigatórios
            if (!await ValidarEnderecoObrigatorio(pessoaDto))
                throw new ValidationException("Na versão 2 da API, pelo menos um endereço é obrigatório para atualizar uma pessoa.");

            // Chama o método da classe base para continuar o processamento
            return await base.UpdateAsync(id, pessoaDto);
        }

        // Implementação dos métodos da interface IPessoaServiceV2
        public Task<bool> ValidarEnderecoObrigatorio(CreatePessoaDto pessoaDto)
        {
            return Task.FromResult(pessoaDto.Enderecos != null && pessoaDto.Enderecos.Any());
        }

        public Task<bool> ValidarEnderecoObrigatorio(UpdatePessoaDto pessoaDto)
        {
            return Task.FromResult(pessoaDto.Enderecos != null && pessoaDto.Enderecos.Any());
        }
    }
}

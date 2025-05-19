using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Application.Interfaces;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Interfaces;

namespace TesteStefanini.Application.Services
{
    public class PessoaService : IPessoaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<Pessoa> _validator;

        public PessoaService(
            IPessoaRepository pessoaRepository,
            IMapper mapper,
            IValidator<Pessoa> validator)
        {
            _pessoaRepository = pessoaRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<PessoaDto>> GetAllAsync()
        {
            var pessoas = await _pessoaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PessoaDto>>(pessoas);
        }

        public async Task<PessoaDto> GetByIdAsync(Guid id)
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null)
                return null;

            return _mapper.Map<PessoaDto>(pessoa);
        }

        public async Task<PessoaDto> CreateAsync(CreatePessoaDto pessoaDto)
        {
            // Mapeia o DTO para a entidade
            var pessoa = _mapper.Map<Pessoa>(pessoaDto);
            
            // Valida a entidade
            var validationResult = await _validator.ValidateAsync(pessoa);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Verifica se o CPF já existe
            if (await _pessoaRepository.CpfJaExisteAsync(pessoa.CPF))
                throw new ValidationException("CPF já cadastrado para outra pessoa.");

            // Persiste a entidade
            await _pessoaRepository.AddAsync(pessoa);
            
            // Retorna o DTO com os dados da entidade criada
            return _mapper.Map<PessoaDto>(pessoa);
        }

        public async Task<PessoaDto> UpdateAsync(Guid id, UpdatePessoaDto pessoaDto)
        {
            // Busca a pessoa pelo ID
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null)
                return null;

            // Atualiza os dados da pessoa
            pessoa.Atualizar(
                pessoaDto.Nome,
                pessoaDto.Sexo,
                pessoaDto.Email,
                pessoaDto.DataNascimento,
                pessoaDto.Naturalidade,
                pessoaDto.Nacionalidade
            );

            // Valida a entidade
            var validationResult = await _validator.ValidateAsync(pessoa);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Persiste as alterações
            await _pessoaRepository.UpdateAsync(pessoa);
            
            // Retorna o DTO com os dados atualizados
            return _mapper.Map<PessoaDto>(pessoa);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Verifica se a pessoa existe
            if (!await _pessoaRepository.ExistsAsync(id))
                return false;

            // Remove a pessoa
            await _pessoaRepository.DeleteAsync(id);
            return true;
        }
    }
}

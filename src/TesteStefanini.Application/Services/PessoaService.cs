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

        public async Task<PagedResult<PessoaDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var pessoas = await _pessoaRepository.GetAllAsync();
            var pessoasDto = _mapper.Map<IEnumerable<PessoaDto>>(pessoas);
            return PagedResult<PessoaDto>.Create(pessoasDto, paginationParams.PageNumber, paginationParams.PageSize);
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

            var pessoa = _mapper.Map<Pessoa>(pessoaDto);
            
            // Valida a entidade
            var validationResult = await _validator.ValidateAsync(pessoa);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);


            if (await _pessoaRepository.CpfJaExisteAsync(pessoa.CPF))
                throw new ValidationException("CPF já cadastrado para outra pessoa.");


            await _pessoaRepository.AddAsync(pessoa);
            

            return _mapper.Map<PessoaDto>(pessoa);
        }

        public async Task<PessoaDto> UpdateAsync(Guid id, UpdatePessoaDto pessoaDto)
        {

            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null)
                return null;


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


            await _pessoaRepository.UpdateAsync(pessoa);
            

            return _mapper.Map<PessoaDto>(pessoa);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            if (!await _pessoaRepository.ExistsAsync(id))
                return false;


            await _pessoaRepository.DeleteAsync(id);
            return true;
        }
    }
}

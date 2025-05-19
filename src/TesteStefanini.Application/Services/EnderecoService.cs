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
    public class EnderecoService : IEnderecoService
    {
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<Endereco> _validator;

        public EnderecoService(
            IEnderecoRepository enderecoRepository,
            IPessoaRepository pessoaRepository,
            IMapper mapper,
            IValidator<Endereco> validator)
        {
            _enderecoRepository = enderecoRepository;
            _pessoaRepository = pessoaRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<EnderecoDto>> GetAllAsync()
        {
            var enderecos = await _enderecoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EnderecoDto>>(enderecos);
        }

        public async Task<EnderecoDto> GetByIdAsync(Guid id)
        {
            var endereco = await _enderecoRepository.GetByIdAsync(id);
            if (endereco == null)
                return null;

            return _mapper.Map<EnderecoDto>(endereco);
        }

        public async Task<IEnumerable<EnderecoDto>> GetByPessoaIdAsync(Guid pessoaId)
        {
            var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoaId);
            return _mapper.Map<IEnumerable<EnderecoDto>>(enderecos);
        }

        public async Task<EnderecoDto> CreateAsync(CreateEnderecoDto enderecoDto)
        {
            // Verifica se a pessoa existe
            if (!await _pessoaRepository.ExistsAsync(enderecoDto.PessoaId))
                throw new ValidationException("Pessoa não encontrada.");

            // Mapeia o DTO para a entidade
            var endereco = _mapper.Map<Endereco>(enderecoDto);
            
            // Valida a entidade
            var validationResult = await _validator.ValidateAsync(endereco);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Persiste a entidade
            await _enderecoRepository.AddAsync(endereco);
            
            // Retorna o DTO com os dados da entidade criada
            return _mapper.Map<EnderecoDto>(endereco);
        }

        public async Task<EnderecoDto> UpdateAsync(Guid id, UpdateEnderecoDto enderecoDto)
        {
            // Busca o endereço pelo ID
            var endereco = await _enderecoRepository.GetByIdAsync(id);
            if (endereco == null)
                return null;

            // Atualiza os dados do endereço
            endereco.Atualizar(
                enderecoDto.Logradouro,
                enderecoDto.Numero,
                enderecoDto.Complemento,
                enderecoDto.Bairro,
                enderecoDto.Cidade,
                enderecoDto.Estado,
                enderecoDto.CEP
            );

            // Valida a entidade
            var validationResult = await _validator.ValidateAsync(endereco);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Persiste as alterações
            await _enderecoRepository.UpdateAsync(endereco);
            
            // Retorna o DTO com os dados atualizados
            return _mapper.Map<EnderecoDto>(endereco);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Verifica se o endereço existe
            if (!await _enderecoRepository.ExistsAsync(id))
                return false;

            // Remove o endereço
            await _enderecoRepository.DeleteAsync(id);
            return true;
        }
    }
}

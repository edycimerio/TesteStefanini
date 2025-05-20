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
    public class PessoaEnderecoService : IPessoaEnderecoService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<Pessoa> _pessoaValidator;
        private readonly IValidator<Endereco> _enderecoValidator;

        public PessoaEnderecoService(
            IPessoaRepository pessoaRepository,
            IEnderecoRepository enderecoRepository,
            IMapper mapper,
            IValidator<Pessoa> pessoaValidator,
            IValidator<Endereco> enderecoValidator)
        {
            _pessoaRepository = pessoaRepository;
            _enderecoRepository = enderecoRepository;
            _mapper = mapper;
            _pessoaValidator = pessoaValidator;
            _enderecoValidator = enderecoValidator;
        }

        public async Task<IEnumerable<PessoaEnderecoDto>> GetAllAsync()
        {
            // Busca todas as pessoas
            var pessoas = await _pessoaRepository.GetAllAsync();
            var pessoasDto = new List<PessoaEnderecoDto>();

            // Para cada pessoa, busca o endereço associado
            foreach (var pessoa in pessoas)
            {
                var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoa.Id);
                var endereco = enderecos.FirstOrDefault();

                var pessoaDto = new PessoaEnderecoDto
                {
                    Id = pessoa.Id,
                    PessoaId = pessoa.Id,
                    Nome = pessoa.Nome,
                    Sexo = pessoa.Sexo,
                    Email = pessoa.Email,
                    DataNascimento = pessoa.DataNascimento,
                    Naturalidade = pessoa.Naturalidade,
                    Nacionalidade = pessoa.Nacionalidade,
                    CPF = pessoa.CPF,
                    DataCadastro = pessoa.DataCadastro,
                    DataAtualizacao = pessoa.DataAtualizacao
                };
                
                if (endereco != null)
                {
                    pessoaDto.Endereco = _mapper.Map<EnderecoDto>(endereco);
                }

                pessoasDto.Add(pessoaDto);
            }

            return pessoasDto;
        }

        public async Task<PagedResult<PessoaEnderecoDto>> GetAllAsync(PaginationParams paginationParams)
        {
            // Busca todas as pessoas
            var pessoas = await _pessoaRepository.GetAllAsync();
            var pessoasDto = new List<PessoaEnderecoDto>();

            // Para cada pessoa, busca o endereço associado
            foreach (var pessoa in pessoas)
            {
                var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoa.Id);
                var endereco = enderecos.FirstOrDefault();

                var pessoaDto = new PessoaEnderecoDto
                {
                    Id = pessoa.Id,
                    PessoaId = pessoa.Id,
                    Nome = pessoa.Nome,
                    Sexo = pessoa.Sexo,
                    Email = pessoa.Email,
                    DataNascimento = pessoa.DataNascimento,
                    Naturalidade = pessoa.Naturalidade,
                    Nacionalidade = pessoa.Nacionalidade,
                    CPF = pessoa.CPF,
                    DataCadastro = pessoa.DataCadastro,
                    DataAtualizacao = pessoa.DataAtualizacao
                };
                
                if (endereco != null)
                {
                    pessoaDto.Endereco = _mapper.Map<EnderecoDto>(endereco);
                }

                pessoasDto.Add(pessoaDto);
            }

            return PagedResult<PessoaEnderecoDto>.Create(pessoasDto, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PessoaEnderecoDto> GetByIdAsync(Guid id)
        {
            // Busca a pessoa pelo ID
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null)
                return null;

            // Busca o endereço associado à pessoa
            var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoa.Id);
            var endereco = enderecos.FirstOrDefault();

            // Mapeia para o DTO
            var pessoaDto = _mapper.Map<PessoaEnderecoDto>(pessoa);
            pessoaDto.PessoaId = pessoa.Id;
            if (endereco != null)
            {
                pessoaDto.Endereco = _mapper.Map<EnderecoDto>(endereco);
            }

            return pessoaDto;
        }

        public async Task<IEnumerable<PessoaEnderecoDto>> GetByPessoaIdAsync(Guid pessoaId)
        {
            // Busca a pessoa pelo ID
            var pessoa = await _pessoaRepository.GetByIdAsync(pessoaId);
            if (pessoa == null)
                return new List<PessoaEnderecoDto>();

            // Busca os endereços associados à pessoa
            var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoaId);
            
            // Se não houver endereços, retorna apenas os dados da pessoa
            if (!enderecos.Any())
            {
                var pessoaSemEnderecoDto = _mapper.Map<PessoaEnderecoDto>(pessoa);
                pessoaSemEnderecoDto.PessoaId = pessoa.Id;
                return new List<PessoaEnderecoDto> { pessoaSemEnderecoDto };
            }

            // Cria um DTO para cada combinação de pessoa e endereço
            var pessoaEnderecoDtos = new List<PessoaEnderecoDto>();
            foreach (var endereco in enderecos)
            {
                var pessoaEnderecoDto = _mapper.Map<PessoaEnderecoDto>(pessoa);
                pessoaEnderecoDto.PessoaId = pessoa.Id;
                pessoaEnderecoDto.Endereco = _mapper.Map<EnderecoDto>(endereco);
                pessoaEnderecoDtos.Add(pessoaEnderecoDto);
            }

            return pessoaEnderecoDtos;
        }

        public async Task<PagedResult<PessoaEnderecoDto>> GetByPessoaIdAsync(Guid pessoaId, PaginationParams paginationParams)
        {
            // Busca a pessoa pelo ID
            var pessoa = await _pessoaRepository.GetByIdAsync(pessoaId);
            if (pessoa == null)
                return new PagedResult<PessoaEnderecoDto>(new List<PessoaEnderecoDto>(), 0, paginationParams.PageNumber, paginationParams.PageSize);

            // Busca os endereços associados à pessoa
            var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoaId);
            
            // Se não houver endereços, retorna apenas os dados da pessoa
            if (!enderecos.Any())
            {
                var pessoaSemEnderecoDto = new PessoaEnderecoDto
                {
                    Id = pessoa.Id,
                    PessoaId = pessoa.Id,
                    Nome = pessoa.Nome,
                    Sexo = pessoa.Sexo,
                    Email = pessoa.Email,
                    DataNascimento = pessoa.DataNascimento,
                    Naturalidade = pessoa.Naturalidade,
                    Nacionalidade = pessoa.Nacionalidade,
                    CPF = pessoa.CPF,
                    DataCadastro = pessoa.DataCadastro,
                    DataAtualizacao = pessoa.DataAtualizacao
                };
                
                return new PagedResult<PessoaEnderecoDto>(new List<PessoaEnderecoDto> { pessoaSemEnderecoDto }, 1, paginationParams.PageNumber, paginationParams.PageSize);
            }

            // Cria um DTO para cada combinação de pessoa e endereço
            var pessoaEnderecoDtos = new List<PessoaEnderecoDto>();
            foreach (var endereco in enderecos)
            {
                var pessoaEnderecoDto = new PessoaEnderecoDto
                {
                    Id = pessoa.Id,
                    PessoaId = pessoa.Id,
                    Nome = pessoa.Nome,
                    Sexo = pessoa.Sexo,
                    Email = pessoa.Email,
                    DataNascimento = pessoa.DataNascimento,
                    Naturalidade = pessoa.Naturalidade,
                    Nacionalidade = pessoa.Nacionalidade,
                    CPF = pessoa.CPF,
                    DataCadastro = pessoa.DataCadastro,
                    DataAtualizacao = pessoa.DataAtualizacao,
                    Endereco = _mapper.Map<EnderecoDto>(endereco)
                };
                
                pessoaEnderecoDtos.Add(pessoaEnderecoDto);
            }

            return PagedResult<PessoaEnderecoDto>.Create(pessoaEnderecoDtos, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PessoaEnderecoDto> CreateAsync(CreatePessoaEnderecoDto pessoaEnderecoDto)
        {
            // Mapeia o DTO para a entidade Pessoa
            var pessoa = new Pessoa(
                pessoaEnderecoDto.Nome,
                pessoaEnderecoDto.Sexo,
                pessoaEnderecoDto.Email,
                pessoaEnderecoDto.DataNascimento,
                pessoaEnderecoDto.Naturalidade,
                pessoaEnderecoDto.Nacionalidade,
                pessoaEnderecoDto.CPF
            );

            // Valida a entidade Pessoa
            var pessoaValidationResult = await _pessoaValidator.ValidateAsync(pessoa);
            if (!pessoaValidationResult.IsValid)
                throw new ValidationException(pessoaValidationResult.Errors);

            // Verifica se o CPF já existe
            if (await _pessoaRepository.CpfJaExisteAsync(pessoa.CPF))
                throw new ValidationException("CPF já cadastrado para outra pessoa.");

            // Persiste a entidade Pessoa
            await _pessoaRepository.AddAsync(pessoa);

            // Cria a entidade Endereco
            var endereco = new Endereco(
                pessoaEnderecoDto.Logradouro,
                pessoaEnderecoDto.Numero,
                pessoaEnderecoDto.Complemento,
                pessoaEnderecoDto.Bairro,
                pessoaEnderecoDto.Cidade,
                pessoaEnderecoDto.Estado,
                pessoaEnderecoDto.CEP,
                pessoa.Id
            );

            // Valida a entidade Endereco
            var enderecoValidationResult = await _enderecoValidator.ValidateAsync(endereco);
            if (!enderecoValidationResult.IsValid)
            {
                // Se houver erro na validação do endereço, remove a pessoa já criada
                await _pessoaRepository.DeleteAsync(pessoa.Id);
                throw new ValidationException(enderecoValidationResult.Errors);
            }

            // Persiste a entidade Endereco
            await _enderecoRepository.AddAsync(endereco);

            // Retorna o DTO com os dados das entidades criadas
            var pessoaEnderecoResultDto = _mapper.Map<PessoaEnderecoDto>(pessoa);
            pessoaEnderecoResultDto.PessoaId = pessoa.Id;
            pessoaEnderecoResultDto.Endereco = _mapper.Map<EnderecoDto>(endereco);

            return pessoaEnderecoResultDto;
        }

        public async Task<PessoaEnderecoDto> UpdateAsync(Guid id, UpdatePessoaEnderecoDto pessoaEnderecoDto)
        {
            // Busca a pessoa pelo ID
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null)
                return null;

            // Atualiza os dados da pessoa
            pessoa.Atualizar(
                pessoaEnderecoDto.Nome,
                pessoaEnderecoDto.Sexo,
                pessoaEnderecoDto.Email,
                pessoaEnderecoDto.DataNascimento,
                pessoaEnderecoDto.Naturalidade,
                pessoaEnderecoDto.Nacionalidade
            );

            // Valida a entidade Pessoa
            var pessoaValidationResult = await _pessoaValidator.ValidateAsync(pessoa);
            if (!pessoaValidationResult.IsValid)
                throw new ValidationException(pessoaValidationResult.Errors);

            // Persiste as alterações na pessoa
            await _pessoaRepository.UpdateAsync(pessoa);

            // Busca o endereço associado à pessoa
            var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoa.Id);
            var endereco = enderecos.FirstOrDefault();

            // Se não existir endereço, cria um novo
            if (endereco == null)
            {
                endereco = new Endereco(
                    pessoaEnderecoDto.Logradouro,
                    pessoaEnderecoDto.Numero,
                    pessoaEnderecoDto.Complemento,
                    pessoaEnderecoDto.Bairro,
                    pessoaEnderecoDto.Cidade,
                    pessoaEnderecoDto.Estado,
                    pessoaEnderecoDto.CEP,
                    pessoa.Id
                );

                // Valida a entidade Endereco
                var enderecoValidationResult = await _enderecoValidator.ValidateAsync(endereco);
                if (!enderecoValidationResult.IsValid)
                    throw new ValidationException(enderecoValidationResult.Errors);

                // Persiste o novo endereço
                await _enderecoRepository.AddAsync(endereco);
            }
            else
            {
                // Atualiza os dados do endereço existente
                endereco.Atualizar(
                    pessoaEnderecoDto.Logradouro,
                    pessoaEnderecoDto.Numero,
                    pessoaEnderecoDto.Complemento,
                    pessoaEnderecoDto.Bairro,
                    pessoaEnderecoDto.Cidade,
                    pessoaEnderecoDto.Estado,
                    pessoaEnderecoDto.CEP
                );

                // Valida a entidade Endereco
                var enderecoValidationResult = await _enderecoValidator.ValidateAsync(endereco);
                if (!enderecoValidationResult.IsValid)
                    throw new ValidationException(enderecoValidationResult.Errors);

                // Persiste as alterações no endereço
                await _enderecoRepository.UpdateAsync(endereco);
            }

            // Retorna o DTO com os dados atualizados
            var pessoaEnderecoResultDto = _mapper.Map<PessoaEnderecoDto>(pessoa);
            pessoaEnderecoResultDto.Endereco = _mapper.Map<EnderecoDto>(endereco);

            return pessoaEnderecoResultDto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Verifica se a pessoa existe
            if (!await _pessoaRepository.ExistsAsync(id))
                return false;

            // Busca os endereços associados à pessoa
            var enderecos = await _enderecoRepository.GetByPessoaIdAsync(id);

            // Remove os endereços
            foreach (var endereco in enderecos)
            {
                await _enderecoRepository.DeleteAsync(endereco.Id);
            }

            // Remove a pessoa
            await _pessoaRepository.DeleteAsync(id);
            return true;
        }
    }
}

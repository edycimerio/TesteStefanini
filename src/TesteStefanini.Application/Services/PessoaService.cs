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
    public class PessoaService : IPessoaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<Pessoa> _validator;
        private readonly IValidator<Endereco> _enderecoValidator;

        public PessoaService(
            IPessoaRepository pessoaRepository,
            IEnderecoRepository enderecoRepository,
            IMapper mapper,
            IValidator<Pessoa> validator,
            IValidator<Endereco> enderecoValidator)
        {
            _pessoaRepository = pessoaRepository;
            _enderecoRepository = enderecoRepository;
            _mapper = mapper;
            _validator = validator;
            _enderecoValidator = enderecoValidator;
        }

        public async Task<IEnumerable<PessoaDto>> GetAllAsync()
        {
            var pessoas = await _pessoaRepository.GetAllAsync();
            var pessoasDto = new List<PessoaDto>();
            
            // Para cada pessoa, buscar seus endereços e adicionar ao DTO
            foreach (var pessoa in pessoas)
            {
                var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoa.Id);
                var pessoaDto = _mapper.Map<PessoaDto>(pessoa);
                pessoaDto.Enderecos = _mapper.Map<List<EnderecoDto>>(enderecos.ToList());
                pessoasDto.Add(pessoaDto);
            }
            
            return pessoasDto;
        }

        public async Task<PagedResult<PessoaDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var pessoas = await _pessoaRepository.GetAllAsync();
            var pessoasDto = new List<PessoaDto>();
            
            // Para cada pessoa, buscar seus endereços e adicionar ao DTO
            foreach (var pessoa in pessoas)
            {
                var enderecos = await _enderecoRepository.GetByPessoaIdAsync(pessoa.Id);
                var pessoaDto = _mapper.Map<PessoaDto>(pessoa);
                pessoaDto.Enderecos = _mapper.Map<List<EnderecoDto>>(enderecos.ToList());
                pessoasDto.Add(pessoaDto);
            }
            
            return PagedResult<PessoaDto>.Create(pessoasDto, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PessoaDto> GetByIdAsync(Guid id)
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null)
                return null;

            // Buscar os endereços associados à pessoa
            var enderecos = await _enderecoRepository.GetByPessoaIdAsync(id);
            
            // Mapear a pessoa para o DTO
            var pessoaDto = _mapper.Map<PessoaDto>(pessoa);
            
            // Adicionar os endereços ao DTO
            pessoaDto.Enderecos = _mapper.Map<List<EnderecoDto>>(enderecos.ToList());
            
            return pessoaDto;
        }

        public virtual async Task<PessoaDto> CreateAsync(CreatePessoaDto pessoaDto)
        {
            // Mapear o DTO para a entidade Pessoa
            var pessoa = _mapper.Map<Pessoa>(pessoaDto);
            
            // Validar a entidade Pessoa
            var validationResult = await _validator.ValidateAsync(pessoa);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Verificar se o CPF já existe
            if (await _pessoaRepository.CpfJaExisteAsync(pessoa.CPF))
                throw new ValidationException("CPF já cadastrado para outra pessoa.");

            // Persistir a pessoa no banco de dados
            await _pessoaRepository.AddAsync(pessoa);
            
            // Lista para armazenar os endereços criados
            var enderecosDto = new List<EnderecoDto>();
            
            // Processar cada endereço na lista (se houver)
            if (pessoaDto.Enderecos != null && pessoaDto.Enderecos.Any())
            {
                foreach (var enderecoDto in pessoaDto.Enderecos)
                {
                    // Criar a entidade Endereco
                    var endereco = new Endereco(
                        enderecoDto.Logradouro,
                        enderecoDto.Numero,
                        enderecoDto.Complemento,
                        enderecoDto.Bairro,
                        enderecoDto.Cidade,
                        enderecoDto.Estado,
                        enderecoDto.CEP,
                        pessoa.Id // Associar o endereço à pessoa recém-criada
                    );
                    
                    // Validar o endereço
                    var enderecoValidationResult = await _enderecoValidator.ValidateAsync(endereco);
                    if (!enderecoValidationResult.IsValid)
                    {
                        // Se houver erro na validação, remover a pessoa já criada
                        await _pessoaRepository.DeleteAsync(pessoa.Id);
                        throw new ValidationException(enderecoValidationResult.Errors);
                    }
                    
                    // Persistir o endereço
                    await _enderecoRepository.AddAsync(endereco);
                    
                    // Adicionar o endereço à lista de DTOs
                    enderecosDto.Add(_mapper.Map<EnderecoDto>(endereco));
                }
            }
            
            // Criar o DTO de resposta
            var pessoaResultDto = _mapper.Map<PessoaDto>(pessoa);
            pessoaResultDto.Enderecos = enderecosDto;
            
            return pessoaResultDto;
        }

        public virtual async Task<PessoaDto> UpdateAsync(Guid id, UpdatePessoaDto pessoaDto)
        {
            // Buscar a pessoa pelo ID
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            if (pessoa == null)
                return null;

            // Atualizar os dados da pessoa
            pessoa.Atualizar(
                pessoaDto.Nome,
                pessoaDto.Sexo,
                pessoaDto.Email,
                pessoaDto.DataNascimento,
                pessoaDto.Naturalidade,
                pessoaDto.Nacionalidade
            );

            // Validar a entidade Pessoa
            var validationResult = await _validator.ValidateAsync(pessoa);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Persistir as alterações na pessoa
            await _pessoaRepository.UpdateAsync(pessoa);
            
            // Buscar os endereços atuais da pessoa
            var enderecosAtuais = await _enderecoRepository.GetByPessoaIdAsync(pessoa.Id);
            var enderecosDto = new List<EnderecoDto>();
            
            // Processar os endereços enviados (se houver)
            if (pessoaDto.Enderecos != null && pessoaDto.Enderecos.Any())
            {
                foreach (var enderecoDto in pessoaDto.Enderecos)
                {
                    // Se o endereço tiver ID, atualizar o existente
                    if (enderecoDto.Id.HasValue && enderecoDto.Id != Guid.Empty)
                    {
                        var enderecoExistente = enderecosAtuais.FirstOrDefault(e => e.Id == enderecoDto.Id);
                        if (enderecoExistente != null)
                        {
                            // Atualizar o endereço existente
                            enderecoExistente.Atualizar(
                                enderecoDto.Logradouro,
                                enderecoDto.Numero,
                                enderecoDto.Complemento,
                                enderecoDto.Bairro,
                                enderecoDto.Cidade,
                                enderecoDto.Estado,
                                enderecoDto.CEP
                            );

                            // Validar o endereço
                            var enderecoValidationResult = await _enderecoValidator.ValidateAsync(enderecoExistente);
                            if (!enderecoValidationResult.IsValid)
                                throw new ValidationException(enderecoValidationResult.Errors);

                            // Persistir as alterações
                            await _enderecoRepository.UpdateAsync(enderecoExistente);
                            enderecosDto.Add(_mapper.Map<EnderecoDto>(enderecoExistente));
                        }
                    }
                    else
                    {
                        // Criar um novo endereço
                        var novoEndereco = new Endereco(
                            enderecoDto.Logradouro,
                            enderecoDto.Numero,
                            enderecoDto.Complemento,
                            enderecoDto.Bairro,
                            enderecoDto.Cidade,
                            enderecoDto.Estado,
                            enderecoDto.CEP,
                            pessoa.Id
                        );

                        // Validar o endereço
                        var enderecoValidationResult = await _enderecoValidator.ValidateAsync(novoEndereco);
                        if (!enderecoValidationResult.IsValid)
                            throw new ValidationException(enderecoValidationResult.Errors);

                        // Persistir o novo endereço
                        await _enderecoRepository.AddAsync(novoEndereco);
                        enderecosDto.Add(_mapper.Map<EnderecoDto>(novoEndereco));
                    }
                }
            }
            
            // Adicionar também os endereços existentes que não foram atualizados
            foreach (var endereco in enderecosAtuais)
            {
                if (!enderecosDto.Any(e => e.Id == endereco.Id))
                {
                    enderecosDto.Add(_mapper.Map<EnderecoDto>(endereco));
                }
            }
            
            // Criar o DTO de resposta
            var pessoaResultDto = _mapper.Map<PessoaDto>(pessoa);
            pessoaResultDto.Enderecos = enderecosDto;
            
            return pessoaResultDto;
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

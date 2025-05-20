using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Application.Services;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Interfaces;
using Xunit;

namespace TesteStefanini.UnitTests.Services
{
    public class PessoaEnderecoServiceTests
    {
        private readonly Mock<IPessoaRepository> _mockPessoaRepository;
        private readonly Mock<IEnderecoRepository> _mockEnderecoRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<Pessoa>> _mockPessoaValidator;
        private readonly Mock<IValidator<Endereco>> _mockEnderecoValidator;
        private readonly PessoaEnderecoService _service;

        public PessoaEnderecoServiceTests()
        {
            _mockPessoaRepository = new Mock<IPessoaRepository>();
            _mockEnderecoRepository = new Mock<IEnderecoRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPessoaValidator = new Mock<IValidator<Pessoa>>();
            _mockEnderecoValidator = new Mock<IValidator<Endereco>>();
            _service = new PessoaEnderecoService(
                _mockPessoaRepository.Object,
                _mockEnderecoRepository.Object,
                _mockMapper.Object,
                _mockPessoaValidator.Object,
                _mockEnderecoValidator.Object);
        }

        // CREATE
        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_DeveCriarPessoaComEndereco()
        {
            // Arrange
            var createDto = new CreatePessoaEnderecoDto
            {
                Nome = "João",
                Sexo = "M",
                Email = "joao@teste.com",
                CPF = "12345678900",
                DataNascimento = DateTime.Now.AddYears(-30),
                Naturalidade = "São Paulo",
                Nacionalidade = "Brasileiro",
                
                // Dados do endereço
                Logradouro = "Rua A",
                Numero = "123",
                Complemento = "Apto 101",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01001-000"
            };

            var pessoaId = Guid.NewGuid();
            var enderecoId = Guid.NewGuid();

            var pessoa = new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900")
            {
                Id = pessoaId
            };

            var endereco = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", pessoaId)
            {
                Id = enderecoId
            };

            var pessoaEnderecoDto = new PessoaEnderecoDto
            {
                Id = pessoaId,
                PessoaId = pessoaId,
                Nome = "João",
                CPF = "12345678900",
                Endereco = new EnderecoDto { 
                    Id = enderecoId, 
                    Logradouro = "Rua A", 
                    Numero = "123", 
                    PessoaId = pessoaId 
                }
            };

            // Configurações do mock para o repositório de pessoa
            _mockPessoaRepository.Setup(r => r.CpfJaExisteAsync(createDto.CPF, null)).ReturnsAsync(false);
            _mockPessoaRepository.Setup(r => r.AddAsync(It.IsAny<Pessoa>()))
                .Callback<Pessoa>(p => p.Id = pessoaId)
                .Returns(Task.CompletedTask);

            // Configurações do mock para o repositório de endereço
            _mockEnderecoRepository.Setup(r => r.AddAsync(It.IsAny<Endereco>()))
                .Callback<Endereco>(e => e.Id = enderecoId)
                .Returns(Task.CompletedTask);

            // Configurações do mock para os validadores
            _mockPessoaValidator.Setup(v => v.ValidateAsync(It.IsAny<Pessoa>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _mockEnderecoValidator.Setup(v => v.ValidateAsync(It.IsAny<Endereco>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Configurações do mock para o mapper
            _mockMapper.Setup(m => m.Map<PessoaEnderecoDto>(It.IsAny<Pessoa>()))
                .Returns(pessoaEnderecoDto);
            _mockMapper.Setup(m => m.Map<EnderecoDto>(It.IsAny<Endereco>()))
                .Returns(pessoaEnderecoDto.Endereco);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pessoaId, result.Id);
            Assert.NotNull(result.Endereco);
        }

        // GET BY ID
        [Fact]
        public async Task GetByIdAsync_QuandoPessoaExiste_DeveRetornarPessoaComEndereco()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var enderecoId = Guid.NewGuid();

            var pessoa = new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900")
            {
                Id = pessoaId
            };

            var endereco = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", pessoaId)
            {
                Id = enderecoId
            };

            var pessoaEnderecoDto = new PessoaEnderecoDto
            {
                Id = pessoaId,
                PessoaId = pessoaId,
                Nome = "João",
                CPF = "12345678900",
                Endereco = new EnderecoDto { 
                    Id = enderecoId, 
                    Logradouro = "Rua A", 
                    Numero = "123", 
                    PessoaId = pessoaId 
                }
            };

            // Configurações do mock para o repositório de pessoa
            _mockPessoaRepository.Setup(r => r.GetByIdAsync(pessoaId)).ReturnsAsync(pessoa);
            
            // Configurações do mock para o repositório de endereço
            _mockEnderecoRepository.Setup(r => r.GetByPessoaIdAsync(pessoaId))
                .ReturnsAsync(new List<Endereco> { endereco });

            // Configurações do mock para o mapper
            _mockMapper.Setup(m => m.Map<PessoaEnderecoDto>(It.IsAny<Pessoa>()))
                .Returns(pessoaEnderecoDto);
            _mockMapper.Setup(m => m.Map<EnderecoDto>(It.IsAny<Endereco>()))
                .Returns(pessoaEnderecoDto.Endereco);

            // Act
            var result = await _service.GetByIdAsync(pessoaId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pessoaId, result.Id);
            Assert.NotNull(result.Endereco);
        }
    }
}

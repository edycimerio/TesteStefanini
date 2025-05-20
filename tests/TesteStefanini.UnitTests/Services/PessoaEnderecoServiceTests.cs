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

        // GET ALL
        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDePessoasComEnderecos()
        {
            // Arrange
            var pessoaId1 = Guid.NewGuid();
            var pessoaId2 = Guid.NewGuid();
            
            var pessoas = new List<Pessoa>
            {
                new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900") { Id = pessoaId1 },
                new Pessoa("Maria", "F", "maria@teste.com", DateTime.Now.AddYears(-25), "Rio de Janeiro", "Brasileira", "98765432100") { Id = pessoaId2 }
            };
            
            var enderecos1 = new List<Endereco>
            {
                new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", pessoaId1)
            };
            
            var enderecos2 = new List<Endereco>
            {
                new Endereco("Rua B", "456", "Casa", "Jardins", "São Paulo", "SP", "01002-000", pessoaId2)
            };
            
            var pessoasEnderecoDto = new List<PessoaEnderecoDto>
            {
                new PessoaEnderecoDto
                {
                    Id = pessoaId1,
                    PessoaId = pessoaId1,
                    Nome = "João",
                    CPF = "12345678900",
                    Endereco = new EnderecoDto { Logradouro = "Rua A", Numero = "123" }
                },
                new PessoaEnderecoDto
                {
                    Id = pessoaId2,
                    PessoaId = pessoaId2,
                    Nome = "Maria",
                    CPF = "98765432100",
                    Endereco = new EnderecoDto { Logradouro = "Rua B", Numero = "456" }
                }
            };
            
            _mockPessoaRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(pessoas);
            _mockEnderecoRepository.Setup(r => r.GetByPessoaIdAsync(pessoaId1)).ReturnsAsync(enderecos1);
            _mockEnderecoRepository.Setup(r => r.GetByPessoaIdAsync(pessoaId2)).ReturnsAsync(enderecos2);
            
            // Act
            var resultado = await _service.GetAllAsync();
            
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _mockPessoaRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }
        
        // GET ALL COM PAGINAÇÃO
        [Fact]
        public async Task GetAllAsync_ComPaginacao_DeveRetornarResultadoPaginado()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var pessoaId1 = Guid.NewGuid();
            var pessoaId2 = Guid.NewGuid();
            var pessoaId3 = Guid.NewGuid();
            
            var pessoas = new List<Pessoa>
            {
                new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900") { Id = pessoaId1 },
                new Pessoa("Maria", "F", "maria@teste.com", DateTime.Now.AddYears(-25), "Rio de Janeiro", "Brasileira", "98765432100") { Id = pessoaId2 },
                new Pessoa("Pedro", "M", "pedro@teste.com", DateTime.Now.AddYears(-40), "Belo Horizonte", "Brasileiro", "11122233344") { Id = pessoaId3 }
            };
            
            var enderecos1 = new List<Endereco>
            {
                new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", pessoaId1)
            };
            
            var enderecos2 = new List<Endereco>
            {
                new Endereco("Rua B", "456", "Casa", "Jardins", "São Paulo", "SP", "01002-000", pessoaId2)
            };
            
            var enderecos3 = new List<Endereco>
            {
                new Endereco("Rua C", "789", "Sala 3", "Bela Vista", "São Paulo", "SP", "01003-000", pessoaId3)
            };
            
            _mockPessoaRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(pessoas);
            _mockEnderecoRepository.Setup(r => r.GetByPessoaIdAsync(pessoaId1)).ReturnsAsync(enderecos1);
            _mockEnderecoRepository.Setup(r => r.GetByPessoaIdAsync(pessoaId2)).ReturnsAsync(enderecos2);
            _mockEnderecoRepository.Setup(r => r.GetByPessoaIdAsync(pessoaId3)).ReturnsAsync(enderecos3);
            
            // Act
            var resultado = await _service.GetAllAsync(paginationParams);
            
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.TotalCount);
            Assert.Equal(3, resultado.Items.Count());
            Assert.Equal(1, resultado.CurrentPage);
            Assert.Equal(10, resultado.PageSize);
            _mockPessoaRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }
        
        // GET BY PESSOA ID COM PAGINAÇÃO
        [Fact]
        public async Task GetByPessoaIdAsync_ComPaginacao_DeveRetornarResultadoPaginado()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            
            var pessoa = new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900") { Id = pessoaId };
            
            var enderecos = new List<Endereco>
            {
                new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", pessoaId),
                new Endereco("Rua B", "456", "Casa", "Jardins", "São Paulo", "SP", "01002-000", pessoaId)
            };
            
            var pessoaDto = new PessoaDto { Id = pessoaId, Nome = "João", CPF = "12345678900" };
            var enderecosDto = new List<EnderecoDto>
            {
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua A", Numero = "123", PessoaId = pessoaId },
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua B", Numero = "456", PessoaId = pessoaId }
            };
            
            _mockPessoaRepository.Setup(r => r.GetByIdAsync(pessoaId)).ReturnsAsync(pessoa);
            _mockEnderecoRepository.Setup(r => r.GetByPessoaIdAsync(pessoaId)).ReturnsAsync(enderecos);
            _mockMapper.Setup(m => m.Map<PessoaDto>(pessoa)).Returns(pessoaDto);
            _mockMapper.Setup(m => m.Map<IEnumerable<EnderecoDto>>(enderecos)).Returns(enderecosDto);
            
            // Act
            var resultado = await _service.GetByPessoaIdAsync(pessoaId, paginationParams);
            
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.TotalCount); // Temos 2 endereços para a pessoa
            Assert.Equal(2, resultado.Items.Count()); // Temos 2 itens na lista
            Assert.Equal(1, resultado.CurrentPage);
            Assert.Equal(10, resultado.PageSize);
            _mockPessoaRepository.Verify(r => r.GetByIdAsync(pessoaId), Times.Once);
            _mockEnderecoRepository.Verify(r => r.GetByPessoaIdAsync(pessoaId), Times.Once);
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

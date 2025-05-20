using System;
using System.Collections.Generic;
using System.Linq;
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
    public class EnderecoServiceTests
    {
        private readonly Mock<IEnderecoRepository> _mockEnderecoRepository;
        private readonly Mock<IPessoaRepository> _mockPessoaRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<Endereco>> _mockValidator;
        private readonly EnderecoService _service;

        public EnderecoServiceTests()
        {
            _mockEnderecoRepository = new Mock<IEnderecoRepository>();
            _mockPessoaRepository = new Mock<IPessoaRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockValidator = new Mock<IValidator<Endereco>>();
            _service = new EnderecoService(
                _mockEnderecoRepository.Object, 
                _mockPessoaRepository.Object, 
                _mockMapper.Object, 
                _mockValidator.Object);
        }

        // GET ALL
        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeEnderecos()
        {
            // Arrange
            var enderecos = new List<Endereco>
            {
                new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", Guid.NewGuid()),
                new Endereco("Rua B", "456", "Casa", "Jardins", "São Paulo", "SP", "01002-000", Guid.NewGuid())
            };

            var enderecosDto = new List<EnderecoDto>
            {
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua A", Numero = "123" },
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua B", Numero = "456" }
            };

            _mockEnderecoRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(enderecos);
            _mockMapper.Setup(m => m.Map<IEnumerable<EnderecoDto>>(enderecos)).Returns(enderecosDto);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockEnderecoRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<EnderecoDto>>(enderecos), Times.Once);
        }

        // GET ALL COM PAGINAÇÃO
        [Fact]
        public async Task GetAllAsync_ComPaginacao_DeveRetornarResultadoPaginado()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var enderecos = new List<Endereco>
            {
                new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", Guid.NewGuid()),
                new Endereco("Rua B", "456", "Casa", "Jardins", "São Paulo", "SP", "01002-000", Guid.NewGuid()),
                new Endereco("Rua C", "789", "Sala 3", "Bela Vista", "São Paulo", "SP", "01003-000", Guid.NewGuid())
            };

            var enderecosDto = new List<EnderecoDto>
            {
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua A", Numero = "123" },
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua B", Numero = "456" },
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua C", Numero = "789" }
            };

            _mockEnderecoRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(enderecos);
            _mockMapper.Setup(m => m.Map<IEnumerable<EnderecoDto>>(enderecos)).Returns(enderecosDto);

            // Act
            var resultado = await _service.GetAllAsync(paginationParams);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.TotalCount);
            Assert.Equal(3, resultado.Items.Count());
            Assert.Equal(1, resultado.CurrentPage);
            Assert.Equal(10, resultado.PageSize);
            _mockEnderecoRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<EnderecoDto>>(enderecos), Times.Once);
        }

        // GET BY PESSOA ID COM PAGINAÇÃO
        [Fact]
        public async Task GetByPessoaIdAsync_ComPaginacao_DeveRetornarResultadoPaginado()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var enderecos = new List<Endereco>
            {
                new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", pessoaId),
                new Endereco("Rua B", "456", "Casa", "Jardins", "São Paulo", "SP", "01002-000", pessoaId)
            };

            var enderecosDto = new List<EnderecoDto>
            {
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua A", Numero = "123", PessoaId = pessoaId },
                new EnderecoDto { Id = Guid.NewGuid(), Logradouro = "Rua B", Numero = "456", PessoaId = pessoaId }
            };

            _mockEnderecoRepository.Setup(r => r.GetByPessoaIdAsync(pessoaId)).ReturnsAsync(enderecos);
            _mockMapper.Setup(m => m.Map<IEnumerable<EnderecoDto>>(enderecos)).Returns(enderecosDto);

            // Act
            var resultado = await _service.GetByPessoaIdAsync(pessoaId, paginationParams);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.TotalCount);
            Assert.Equal(2, resultado.Items.Count());
            Assert.Equal(1, resultado.CurrentPage);
            Assert.Equal(10, resultado.PageSize);
            _mockEnderecoRepository.Verify(r => r.GetByPessoaIdAsync(pessoaId), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<EnderecoDto>>(enderecos), Times.Once);
        }

        // GET BY ID
        [Fact]
        public async Task GetByIdAsync_QuandoEnderecoExiste_DeveRetornarEndereco()
        {
            // Arrange
            var id = Guid.NewGuid();
            var endereco = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", Guid.NewGuid())
            {
                Id = id
            };

            var enderecoDto = new EnderecoDto { Id = id, Logradouro = "Rua A", Numero = "123" };

            _mockEnderecoRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(endereco);
            _mockMapper.Setup(m => m.Map<EnderecoDto>(endereco)).Returns(enderecoDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Rua A", result.Logradouro);
        }

        // CREATE
        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_DeveCriarEndereco()
        {
            // Arrange
            var pessoaId = Guid.NewGuid();
            var createDto = new CreateEnderecoDto
            {
                Logradouro = "Rua A",
                Numero = "123",
                Complemento = "Apto 101",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01001-000",
                PessoaId = pessoaId
            };

            var endereco = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", pessoaId)
            {
                Id = Guid.NewGuid()
            };

            var enderecoDto = new EnderecoDto 
            { 
                Id = endereco.Id, 
                Logradouro = "Rua A", 
                Numero = "123", 
                PessoaId = pessoaId 
            };

            _mockPessoaRepository.Setup(r => r.ExistsAsync(pessoaId)).ReturnsAsync(true);
            _mockMapper.Setup(m => m.Map<Endereco>(createDto)).Returns(endereco);
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Endereco>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _mockEnderecoRepository.Setup(r => r.AddAsync(endereco)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<EnderecoDto>(endereco)).Returns(enderecoDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(endereco.Id, result.Id);
            Assert.Equal("Rua A", result.Logradouro);
        }

        // UPDATE
        [Fact]
        public async Task UpdateAsync_QuandoDadosValidos_DeveAtualizarEndereco()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pessoaId = Guid.NewGuid();
            var updateDto = new UpdateEnderecoDto
            {
                Logradouro = "Rua A Atualizada",
                Numero = "123",
                Complemento = "Apto 101",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01001-000"
            };

            var enderecoExistente = new Endereco("Rua A", "123", "Apto 101", "Centro", "São Paulo", "SP", "01001-000", pessoaId)
            {
                Id = id
            };

            var enderecoDto = new EnderecoDto 
            { 
                Id = id, 
                Logradouro = "Rua A Atualizada", 
                Numero = "123", 
                PessoaId = pessoaId 
            };

            _mockEnderecoRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(enderecoExistente);
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Endereco>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _mockEnderecoRepository.Setup(r => r.UpdateAsync(It.IsAny<Endereco>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<EnderecoDto>(It.IsAny<Endereco>())).Returns(enderecoDto);

            // Act
            var result = await _service.UpdateAsync(id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Rua A Atualizada", result.Logradouro);
        }

        // DELETE
        [Fact]
        public async Task DeleteAsync_QuandoEnderecoExiste_DeveRetornarTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockEnderecoRepository.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
            _mockEnderecoRepository.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.True(result);
        }
    }
}

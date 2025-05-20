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
    public class PessoaServiceTests
    {
        private readonly Mock<IPessoaRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<Pessoa>> _mockValidator;
        private readonly PessoaService _service;

        public PessoaServiceTests()
        {
            _mockRepository = new Mock<IPessoaRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockValidator = new Mock<IValidator<Pessoa>>();
            _service = new PessoaService(_mockRepository.Object, _mockMapper.Object, _mockValidator.Object);
        }

        // GET ALL
        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDePessoas()
        {
            // Arrange
            var pessoas = new List<Pessoa>
            {
                new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900"),
                new Pessoa("Maria", "F", "maria@teste.com", DateTime.Now.AddYears(-25), "Rio de Janeiro", "Brasileira", "98765432100")
            };

            var pessoasDto = new List<PessoaDto>
            {
                new PessoaDto { Id = Guid.NewGuid(), Nome = "João", CPF = "12345678900" },
                new PessoaDto { Id = Guid.NewGuid(), Nome = "Maria", CPF = "98765432100" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(pessoas);
            _mockMapper.Setup(m => m.Map<IEnumerable<PessoaDto>>(pessoas)).Returns(pessoasDto);

            // Act
            var resultado = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<PessoaDto>>(pessoas), Times.Once);
        }

        // GET ALL COM PAGINAÇÃO
        [Fact]
        public async Task GetAllAsync_ComPaginacao_DeveRetornarResultadoPaginado()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var pessoas = new List<Pessoa>
            {
                new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900"),
                new Pessoa("Maria", "F", "maria@teste.com", DateTime.Now.AddYears(-25), "Rio de Janeiro", "Brasileira", "98765432100"),
                new Pessoa("Pedro", "M", "pedro@teste.com", DateTime.Now.AddYears(-40), "Belo Horizonte", "Brasileiro", "11122233344")
            };

            var pessoasDto = new List<PessoaDto>
            {
                new PessoaDto { Id = Guid.NewGuid(), Nome = "João", CPF = "12345678900" },
                new PessoaDto { Id = Guid.NewGuid(), Nome = "Maria", CPF = "98765432100" },
                new PessoaDto { Id = Guid.NewGuid(), Nome = "Pedro", CPF = "11122233344" }
            };

            // Não precisamos criar um PagedResult manualmente, pois o serviço o criará

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(pessoas);
            _mockMapper.Setup(m => m.Map<IEnumerable<PessoaDto>>(pessoas)).Returns(pessoasDto);

            // Act
            var resultado = await _service.GetAllAsync(paginationParams);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.TotalCount);
            Assert.Equal(3, resultado.Items.Count());
            Assert.Equal(1, resultado.CurrentPage);
            Assert.Equal(10, resultado.PageSize);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<PessoaDto>>(pessoas), Times.Once);
        }

        // GET BY ID
        [Fact]
        public async Task GetByIdAsync_QuandoPessoaExiste_DeveRetornarPessoa()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pessoa = new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900")
            {
                Id = id
            };

            var pessoaDto = new PessoaDto { Id = id, Nome = "João", CPF = "12345678900" };

            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(pessoa);
            _mockMapper.Setup(m => m.Map<PessoaDto>(pessoa)).Returns(pessoaDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("João", result.Nome);
        }

        // CREATE
        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_DeveCriarPessoa()
        {
            // Arrange
            var createDto = new CreatePessoaDto
            {
                Nome = "João",
                Sexo = "M",
                Email = "joao@teste.com",
                CPF = "12345678900",
                DataNascimento = DateTime.Now.AddYears(-30),
                Naturalidade = "São Paulo",
                Nacionalidade = "Brasileiro"
            };

            var pessoa = new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900")
            {
                Id = Guid.NewGuid()
            };

            var pessoaDto = new PessoaDto { Id = pessoa.Id, Nome = "João", CPF = "12345678900" };

            _mockMapper.Setup(m => m.Map<Pessoa>(createDto)).Returns(pessoa);
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Pessoa>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _mockRepository.Setup(r => r.CpfJaExisteAsync(createDto.CPF, null)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(pessoa)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<PessoaDto>(pessoa)).Returns(pessoaDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pessoa.Id, result.Id);
            Assert.Equal("João", result.Nome);
        }

        // UPDATE
        [Fact]
        public async Task UpdateAsync_QuandoDadosValidos_DeveAtualizarPessoa()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateDto = new UpdatePessoaDto
            {
                Nome = "João Atualizado",
                Sexo = "M",
                Email = "joao@teste.com",
                DataNascimento = DateTime.Now.AddYears(-30),
                Naturalidade = "São Paulo",
                Nacionalidade = "Brasileiro"
            };

            var pessoaExistente = new Pessoa("João", "M", "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasileiro", "12345678900")
            {
                Id = id
            };

            var pessoaDto = new PessoaDto { Id = id, Nome = "João Atualizado" };

            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(pessoaExistente);
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Pessoa>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Pessoa>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<PessoaDto>(It.IsAny<Pessoa>())).Returns(pessoaDto);

            // Act
            var result = await _service.UpdateAsync(id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("João Atualizado", result.Nome);
        }

        // DELETE
        [Fact]
        public async Task DeleteAsync_QuandoPessoaExiste_DeveRetornarTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.True(result);
        }
    }
}

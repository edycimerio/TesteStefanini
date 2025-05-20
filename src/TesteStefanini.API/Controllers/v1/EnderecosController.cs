using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Application.Interfaces;

namespace TesteStefanini.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/enderecos")]
    [Authorize]
    public class EnderecosController : ControllerBase
    {
        private readonly IEnderecoService _enderecoService;

        public EnderecosController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        // GET: api/v1/enderecos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnderecoDto>>> GetAll([FromQuery] PaginationParams paginationParams = null)
        {
            if (paginationParams == null)
            {
                var enderecos = await _enderecoService.GetAllAsync();
                return Ok(enderecos);
            }
            else
            {
                var enderecosPaginados = await _enderecoService.GetAllAsync(paginationParams);
                return Ok(enderecosPaginados);
            }
        }

        // GET: api/v1/enderecos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EnderecoDto>> GetById(Guid id)
        {
            var endereco = await _enderecoService.GetByIdAsync(id);
            if (endereco == null)
                return NotFound(new { message = "Endereço não encontrado" });

            return Ok(endereco);
        }

        // GET: api/v1/enderecos/pessoa/{pessoaId}
        [HttpGet("pessoa/{pessoaId}")]
        public async Task<ActionResult<IEnumerable<EnderecoDto>>> GetByPessoaId(Guid pessoaId, [FromQuery] PaginationParams paginationParams = null)
        {
            if (paginationParams == null)
            {
                var enderecos = await _enderecoService.GetByPessoaIdAsync(pessoaId);
                return Ok(enderecos);
            }
            else
            {
                var enderecosPaginados = await _enderecoService.GetByPessoaIdAsync(pessoaId, paginationParams);
                return Ok(enderecosPaginados);
            }
        }

        // POST: api/v1/enderecos
        [HttpPost]
        public async Task<ActionResult<EnderecoDto>> Create([FromBody] CreateEnderecoDto enderecoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var novoEndereco = await _enderecoService.CreateAsync(enderecoDto);
                return CreatedAtAction(nameof(GetById), new { id = novoEndereco.Id, version = "1.0" }, novoEndereco);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar endereço", error = ex.Message });
            }
        }

        // PUT: api/v1/enderecos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEnderecoDto enderecoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var enderecoAtualizado = await _enderecoService.UpdateAsync(id, enderecoDto);
                if (enderecoAtualizado == null)
                    return NotFound(new { message = "Endereço não encontrado" });

                return Ok(enderecoAtualizado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar endereço", error = ex.Message });
            }
        }

        // DELETE: api/v1/enderecos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var resultado = await _enderecoService.DeleteAsync(id);
                if (!resultado)
                    return NotFound(new { message = "Endereço não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir endereço", error = ex.Message });
            }
        }
    }
}

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
    [Route("api/v{version:apiVersion}/pessoas-enderecos")]
    [Authorize]
    public class PessoasEnderecosController : ControllerBase
    {
        private readonly IPessoaEnderecoService _pessoaEnderecoService;

        public PessoasEnderecosController(IPessoaEnderecoService pessoaEnderecoService)
        {
            _pessoaEnderecoService = pessoaEnderecoService;
        }

        // GET: api/v1/pessoas-enderecos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaEnderecoDto>>> GetAll()
        {
            var pessoasEnderecos = await _pessoaEnderecoService.GetAllAsync();
            return Ok(pessoasEnderecos);
        }

        // GET: api/v1/pessoas-enderecos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaEnderecoDto>> GetById(Guid id)
        {
            var pessoaEndereco = await _pessoaEnderecoService.GetByIdAsync(id);
            if (pessoaEndereco == null)
                return NotFound(new { message = "Registro não encontrado" });

            return Ok(pessoaEndereco);
        }

        // GET: api/v1/pessoas-enderecos/pessoa/{pessoaId}
        [HttpGet("pessoa/{pessoaId}")]
        public async Task<ActionResult<IEnumerable<PessoaEnderecoDto>>> GetByPessoaId(Guid pessoaId)
        {
            var pessoaEnderecos = await _pessoaEnderecoService.GetByPessoaIdAsync(pessoaId);
            return Ok(pessoaEnderecos);
        }

        // POST: api/v1/pessoas-enderecos
        [HttpPost]
        public async Task<ActionResult<PessoaEnderecoDto>> Create([FromBody] CreatePessoaEnderecoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var novoPessoaEndereco = await _pessoaEnderecoService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = novoPessoaEndereco.Id, version = "1.0" }, novoPessoaEndereco);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar registro", error = ex.Message });
            }
        }

        // PUT: api/v1/pessoas-enderecos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePessoaEnderecoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var atualizado = await _pessoaEnderecoService.UpdateAsync(id, dto);
                if (atualizado == null)
                    return NotFound(new { message = "Registro não encontrado" });

                return Ok(atualizado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar registro", error = ex.Message });
            }
        }

        // DELETE: api/v1/pessoas-enderecos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var resultado = await _pessoaEnderecoService.DeleteAsync(id);
                if (!resultado)
                    return NotFound(new { message = "Registro não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir registro", error = ex.Message });
            }
        }
    }
}

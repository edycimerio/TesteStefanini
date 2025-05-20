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
    [Route("api/v{version:apiVersion}/pessoas")]
    [Authorize]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;

        public PessoasController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        // GET: api/v1/pessoas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaDto>>> GetAll()
        {
            var pessoas = await _pessoaService.GetAllAsync();
            return Ok(pessoas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDto>> GetById(Guid id)
        {
            var pessoa = await _pessoaService.GetByIdAsync(id);
            if (pessoa == null)
                return NotFound(new { message = "Pessoa não encontrada" });

            return Ok(pessoa);
        }


        [HttpPost]
        public async Task<ActionResult<PessoaDto>> Create([FromBody] CreatePessoaDto pessoaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var novaPessoa = await _pessoaService.CreateAsync(pessoaDto);
                return CreatedAtAction(nameof(GetById), new { id = novaPessoa.Id, version = "1.0" }, novaPessoa);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar pessoa", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePessoaDto pessoaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var pessoaAtualizada = await _pessoaService.UpdateAsync(id, pessoaDto);
                if (pessoaAtualizada == null)
                    return NotFound(new { message = "Pessoa não encontrada" });

                return Ok(pessoaAtualizada);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar pessoa", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var resultado = await _pessoaService.DeleteAsync(id);
                if (!resultado)
                    return NotFound(new { message = "Pessoa não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir pessoa", error = ex.Message });
            }
        }
    }
}

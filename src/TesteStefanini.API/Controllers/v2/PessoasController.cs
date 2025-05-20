using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Application.Interfaces;

namespace TesteStefanini.API.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/pessoas")]
    [Authorize]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;
        private readonly IPessoaEnderecoService _pessoaEnderecoService;

        public PessoasController(IPessoaService pessoaService, IPessoaEnderecoService pessoaEnderecoService)
        {
            _pessoaService = pessoaService;
            _pessoaEnderecoService = pessoaEnderecoService;
        }

        // GET: api/v2/pessoas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaDto>>> GetAll()
        {
            var pessoas = await _pessoaService.GetAllAsync();
            return Ok(pessoas);
        }

        // GET: api/v2/pessoas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDto>> GetById(Guid id)
        {
            var pessoa = await _pessoaService.GetByIdAsync(id);
            if (pessoa == null)
                return NotFound(new { message = "Pessoa não encontrada" });

            return Ok(pessoa);
        }

        // GET: api/v2/pessoas/{id}/completo
        [HttpGet("{id}/completo")]
        public async Task<ActionResult<PessoaEnderecoDto>> GetPessoaCompletoById(Guid id)
        {
            var pessoaCompleta = await _pessoaEnderecoService.GetByPessoaIdAsync(id);
            if (pessoaCompleta == null || !pessoaCompleta.Any())
                return NotFound(new { message = "Pessoa não encontrada ou sem endereços cadastrados" });

            return Ok(pessoaCompleta);
        }

        // POST: api/v2/pessoas
        [HttpPost]
        public async Task<ActionResult<PessoaDto>> Create([FromBody] CreatePessoaDto pessoaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var novaPessoa = await _pessoaService.CreateAsync(pessoaDto);
                return CreatedAtAction(nameof(GetById), new { id = novaPessoa.Id, version = "2.0" }, novaPessoa);
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

        // POST: api/v2/pessoas/completo
        [HttpPost("completo")]
        public async Task<ActionResult<PessoaEnderecoDto>> CreateCompleto([FromBody] CreatePessoaEnderecoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var novaPessoaEndereco = await _pessoaEnderecoService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetPessoaCompletoById), new { id = novaPessoaEndereco.PessoaId, version = "2.0" }, novaPessoaEndereco);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar pessoa com endereço", error = ex.Message });
            }
        }

        // PUT: api/v2/pessoas/{id}
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

        // DELETE: api/v2/pessoas/{id}
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

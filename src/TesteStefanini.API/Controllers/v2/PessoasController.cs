using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IPessoaServiceV2 _pessoaService;

        public PessoasController(IPessoaServiceV2 pessoaService)
        {
            _pessoaService = pessoaService;
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
                return Created($"/api/v2/pessoas/{novaPessoa.Id}", novaPessoa);
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


    }
}

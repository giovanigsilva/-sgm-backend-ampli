using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using syspublicidade.prefeitura.Application.DTOs;
using syspublicidade.prefeitura.Application.features.UsuariosM.Commands;
using syspublicidade.prefeitura.Application.features.UsuariosM.Queries;

namespace syspublicidade.prefeitura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UsuariosDto>> Criar([FromBody] CriarUsuarioDto dto, CancellationToken ct)
        => await mediator.SendCommandAsync<CriarUsuarioCommand, UsuariosDto>(new CriarUsuarioCommand(dto), ct);

    [HttpPut]
    public async Task<ActionResult<UsuariosDto>> Atualizar([FromBody] AtualizarUsuarioDto dto, CancellationToken ct)
        => await mediator.SendCommandAsync<AtualizarUsuarioCommand, UsuariosDto>(new AtualizarUsuarioCommand(dto), ct);

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Deletar(Guid id, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<DeletarUsuarioCommand, bool>(new DeletarUsuarioCommand(id), ct);
        return result ? Ok() : NotFound();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuariosDto?>> GetById(Guid id, CancellationToken ct)
        => await mediator.SendQueryAsync<GetUsuarioByIdQuery, UsuariosDto?>(new GetUsuarioByIdQuery(id), ct);

    [HttpGet]
    public async Task<ActionResult<List<UsuariosDto>>> Listar(CancellationToken ct)
        => await mediator.SendQueryAsync<ListarUsuariosQuery, List<UsuariosDto>>(new ListarUsuariosQuery(), ct);
}

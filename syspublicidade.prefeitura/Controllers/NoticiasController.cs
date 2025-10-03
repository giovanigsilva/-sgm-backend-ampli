using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using syspublicidade.prefeitura.Application.DTOs;
using syspublicidade.prefeitura.Application.features.NoticiasM.Commands;
using syspublicidade.prefeitura.Application.features.NoticiasM.Queries;

namespace syspublicidade.prefeitura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoticiasController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NoticiasDto>> Criar([FromBody] CriarNoticiaDto dto, CancellationToken ct)
        => await mediator.SendCommandAsync<CriarNoticiaCommand, NoticiasDto>(new CriarNoticiaCommand(dto), ct);

    [HttpPut]
    public async Task<ActionResult<NoticiasDto>> Atualizar([FromBody] AtualizarNoticiaDto dto, CancellationToken ct)
        => await mediator.SendCommandAsync<AtualizarNoticiaCommand, NoticiasDto>(new AtualizarNoticiaCommand(dto), ct);

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Deletar(Guid id, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<DeletarNoticiaCommand, bool>(new DeletarNoticiaCommand(id), ct);
        return result ? Ok() : NotFound();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NoticiasDto?>> GetById(Guid id, CancellationToken ct)
        => await mediator.SendQueryAsync<GetNoticiaByIdQuery, NoticiasDto?>(new GetNoticiaByIdQuery(id), ct);

    [HttpGet]
    public async Task<ActionResult<List<NoticiasDto>>> Listar(CancellationToken ct)
        => await mediator.SendQueryAsync<ListarNoticiasQuery, List<NoticiasDto>>(new ListarNoticiasQuery(), ct);

    [HttpGet("ultimas")]
    public async Task<ActionResult<Dictionary<string, List<NoticiasDto>>>> ListarUltimas(CancellationToken ct)
        => await mediator.SendQueryAsync<ListarUltimasNoticiasQuery, Dictionary<string, List<NoticiasDto>>>(new ListarUltimasNoticiasQuery(), ct);
}

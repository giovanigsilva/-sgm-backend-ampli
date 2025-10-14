using Cortex.Mediator.Queries;
using syspublicidade.prefeitura.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syspublicidade.prefeitura.Application.features.NoticiasM.Queries
{
    public sealed record GetNoticiaByIdQuery(Guid Id) : IQuery<NoticiasDto?>;
    public sealed record ListarNoticiasQuery() : IQuery<List<NoticiasDto>>;
    public sealed record ListarUltimasNoticiasQuery() : IQuery<Dictionary<string, List<NoticiasDto>>>;
    public sealed record GetNoticiasUltimos7DiasQuery() : IQuery<List<NoticiasPorDiaDto>>;
}

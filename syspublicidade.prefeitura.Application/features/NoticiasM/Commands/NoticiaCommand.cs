using Cortex.Mediator.Commands;
using syspublicidade.prefeitura.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syspublicidade.prefeitura.Application.features.NoticiasM.Commands
{
    public sealed record CriarNoticiaCommand(CriarNoticiaDto Noticia) : ICommand<NoticiasDto>;
    public sealed record AtualizarNoticiaCommand(AtualizarNoticiaDto Noticia) : ICommand<NoticiasDto>;
    public sealed record DeletarNoticiaCommand(Guid Id) : ICommand<bool>;
}

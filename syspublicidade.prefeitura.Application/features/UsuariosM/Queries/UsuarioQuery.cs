using Cortex.Mediator.Queries;
using syspublicidade.prefeitura.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syspublicidade.prefeitura.Application.features.UsuariosM.Queries
{
    public sealed record GetUsuarioByIdQuery(Guid Id) : IQuery<UsuariosDto?>;
    public sealed record ListarUsuariosQuery() : IQuery<List<UsuariosDto>>;

}

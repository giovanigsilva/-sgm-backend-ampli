using Cortex.Mediator.Commands;
using syspublicidade.prefeitura.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syspublicidade.prefeitura.Application.features.UsuariosM.Commands
{
   public sealed record CriarUsuarioCommand(CriarUsuarioDto Usuario) : ICommand<UsuariosDto>;
    public sealed record AtualizarUsuarioCommand(AtualizarUsuarioDto Usuario) : ICommand<UsuariosDto>;
    public sealed record DeletarUsuarioCommand(Guid Id) : ICommand<bool>;
}

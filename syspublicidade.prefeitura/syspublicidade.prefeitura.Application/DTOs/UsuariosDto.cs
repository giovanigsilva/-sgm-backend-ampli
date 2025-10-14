using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syspublicidade.prefeitura.Application.DTOs
{
    public record CriarUsuarioDto(string Nome, string Email, string Senha, bool IsAdmin);
    public record AtualizarUsuarioDto(Guid Id, string Nome, string Email, bool IsAdmin);
    public record UsuariosDto(Guid Id, string Nome, string Email, bool IsAdmin);
}

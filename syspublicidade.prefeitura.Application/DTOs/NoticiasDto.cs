using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syspublicidade.prefeitura.Application.DTOs
{
    public sealed record NoticiasDto(
    Guid Id,
    string Titulo,
    string Conteudo,
    string Categoria,
    DateTime CriadoEm,
    string Autor
);

    // DTO usado no Create
    public sealed record CriarNoticiaDto(
        string Titulo,
        string Conteudo,
        string Categoria,
        Guid UsuarioId
    );

    // DTO usado no Update
    public sealed record AtualizarNoticiaDto(
        Guid Id,
        string Titulo,
        string Conteudo,
        string Categoria
    );
}

using System;

namespace syspublicidade.prefeitura.Application.DTOs
{
    public sealed record NoticiasDto(
        Guid Id,
        string Titulo,
        string Conteudo,
        string Categoria,
        DateTime CriadoEm,
        string Autor,
        string? CaminhoFoto = null
    );

    // DTO usado no Create
    public sealed record CriarNoticiaDto(
        string Titulo,
        string Conteudo,
        string Categoria,
        Guid UsuarioId,
       string? CaminhoFoto = null
    );

    // DTO usado no Update
    public sealed record AtualizarNoticiaDto(
        Guid Id,
        string Titulo,
        string Conteudo,
        string Categoria,
        string? CaminhoFoto = null);

    public sealed record NoticiasPorDiaDto(string Dia, int Total);
}

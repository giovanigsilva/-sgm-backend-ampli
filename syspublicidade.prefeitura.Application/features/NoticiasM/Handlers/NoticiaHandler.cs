using Cortex.Mediator.Commands;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using syspublicidade.prefeitura.Application.DTOs;
using syspublicidade.prefeitura.Application.features.NoticiasM.Commands;
using syspublicidade.prefeitura.Application.features.NoticiasM.Queries;
using syspublicidade.prefeitura.Domain.Entities;
using syspublicidade.prefeitura.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syspublicidade.prefeitura.Application.features.NoticiasM.Handlers
{
    public sealed class CriarNoticiaHandler(AppDbContext db)
    : ICommandHandler<CriarNoticiaCommand, NoticiasDto>
    {
        public async Task<NoticiasDto> Handle(CriarNoticiaCommand command, CancellationToken ct)
        {
            var usuario = await db.Usuarios.FindAsync(new object?[] { command.Noticia.UsuarioId }, ct);
            if (usuario is null) throw new Exception("Usuário não encontrado");

            Enum.TryParse<CategoriaNoticia>(command.Noticia.Categoria, out var categoria);

            var entity = new Noticias
            {
                Id = Guid.NewGuid(),
                Titulo = command.Noticia.Titulo,
                Conteudo = command.Noticia.Conteudo,
                Categoria = categoria,
                UsuarioId = usuario.Id
            };

            db.Noticias.Add(entity);
            await db.SaveChangesAsync(ct);

            return new NoticiasDto(entity.Id, entity.Titulo, entity.Conteudo, entity.Categoria.ToString(), entity.CriadoEm, usuario.Nome);
        }
    }
    public sealed class AtualizarNoticiaHandler(AppDbContext db)
    : ICommandHandler<AtualizarNoticiaCommand, NoticiasDto>
    {
        public async Task<NoticiasDto> Handle(AtualizarNoticiaCommand command, CancellationToken ct)
        {
            var noticia = await db.Noticias.FindAsync(new object?[] { command.Noticia.Id }, ct);
            if (noticia is null) throw new Exception("Notícia não encontrada");

            Enum.TryParse<CategoriaNoticia>(command.Noticia.Categoria, out var categoria);

            noticia.Titulo = command.Noticia.Titulo;
            noticia.Conteudo = command.Noticia.Conteudo;
            noticia.Categoria = categoria;

            await db.SaveChangesAsync(ct);

            var usuario = await db.Usuarios.FindAsync(new object?[] { noticia.UsuarioId }, ct);

            return new NoticiasDto(noticia.Id, noticia.Titulo, noticia.Conteudo, noticia.Categoria.ToString(), noticia.CriadoEm, usuario?.Nome ?? "");
        }
    }
    public sealed class DeletarNoticiaHandler(AppDbContext db)
    : ICommandHandler<DeletarNoticiaCommand, bool>
    {
        public async Task<bool> Handle(DeletarNoticiaCommand command, CancellationToken ct)
        {
            var noticia = await db.Noticias.FindAsync(new object?[] { command.Id }, ct);
            if (noticia is null) return false;

            db.Noticias.Remove(noticia);
            await db.SaveChangesAsync(ct);

            return true;
        }
    }

    public sealed class GetNoticiaByIdHandler(AppDbContext db)
    : IQueryHandler<GetNoticiaByIdQuery, NoticiasDto?>
    {
        public async Task<NoticiasDto?> Handle(GetNoticiaByIdQuery query, CancellationToken ct)
        {
            var noticia = await db.Noticias.FindAsync(new object?[] { query.Id }, ct);
            if (noticia is null) return null;

            var usuario = await db.Usuarios.FindAsync(new object?[] { noticia.UsuarioId }, ct);

            return new NoticiasDto(noticia.Id, noticia.Titulo, noticia.Conteudo, noticia.Categoria.ToString(), noticia.CriadoEm, usuario?.Nome ?? "");
        }
    }
    public sealed class ListarNoticiasHandler(AppDbContext db)
    : IQueryHandler<ListarNoticiasQuery, List<NoticiasDto>>
    {
        public async Task<List<NoticiasDto>> Handle(ListarNoticiasQuery query, CancellationToken ct)
        {
            return await db.Noticias
                .Include(n => n.Usuario)
                .Select(n => new NoticiasDto(
                    n.Id,
                    n.Titulo,
                    n.Conteudo,
                    n.Categoria.ToString(),
                    n.CriadoEm,
                    n.Usuario.Nome))
                .ToListAsync(ct);
        }
    }
    public sealed class ListarUltimasNoticiasHandler(AppDbContext db)
    : IQueryHandler<ListarUltimasNoticiasQuery, Dictionary<string, List<NoticiasDto>>>
    {
        public async Task<Dictionary<string, List<NoticiasDto>>> Handle(ListarUltimasNoticiasQuery query, CancellationToken ct)
        {
            var result = new Dictionary<string, List<NoticiasDto>>();

            foreach (CategoriaNoticia categoria in Enum.GetValues(typeof(CategoriaNoticia)))
            {
                var noticias = await db.Noticias
                    .Include(n => n.Usuario)
                    .Where(n => n.Categoria == categoria)
                    .OrderByDescending(n => n.CriadoEm)
                    .Take(10)
                    .Select(n => new NoticiasDto(
                        n.Id,
                        n.Titulo,
                        n.Conteudo,
                        n.Categoria.ToString(),
                        n.CriadoEm,
                        n.Usuario.Nome))
                    .ToListAsync(ct);

                result[categoria.ToString()] = noticias;
            }

            return result;
        }
    }
}

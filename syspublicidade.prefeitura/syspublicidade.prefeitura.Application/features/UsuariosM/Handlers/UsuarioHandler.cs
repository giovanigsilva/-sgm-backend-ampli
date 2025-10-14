using Cortex.Mediator.Commands;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using syspublicidade.prefeitura.Application.DTOs;
using syspublicidade.prefeitura.Application.features.UsuariosM.Commands;
using syspublicidade.prefeitura.Application.features.UsuariosM.Queries;
using syspublicidade.prefeitura.Domain.Entities;
using syspublicidade.prefeitura.Infrastructure;

namespace syspublicidade.prefeitura.Application.features.UsuariosM.Handlers
{
    public sealed class AdicionarUsuarioHandler(AppDbContext db)
     : ICommandHandler<CriarUsuarioCommand, UsuariosDto>
    {
        public async Task<UsuariosDto> Handle(CriarUsuarioCommand command, CancellationToken ct)
        {
            var entity = new Usuarios
            {
                Id = Guid.NewGuid(),
                Nome = command.Usuario.Nome,
                Email = command.Usuario.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(command.Usuario.Senha),
                IsAdmin = command.Usuario.IsAdmin
            };

            db.Usuarios.Add(entity);
            await db.SaveChangesAsync(ct);

            return new UsuariosDto(entity.Id, entity.Nome, entity.Email, entity.IsAdmin);
        }
    }
    public sealed class AtualizarUsuarioHandler(AppDbContext db)
    : ICommandHandler<AtualizarUsuarioCommand, UsuariosDto>
    {
        public async Task<UsuariosDto> Handle(AtualizarUsuarioCommand command, CancellationToken ct)
        {
            var usuario = await db.Usuarios.FindAsync(new object?[] { command.Usuario.Id }, ct);
            if (usuario is null) throw new Exception("Usuário não encontrado");

            usuario.Nome = command.Usuario.Nome;
            usuario.Email = command.Usuario.Email;
            usuario.IsAdmin = command.Usuario.IsAdmin;

            await db.SaveChangesAsync(ct);

            return new UsuariosDto(usuario.Id, usuario.Nome, usuario.Email, usuario.IsAdmin);
        }
    }
    public sealed class DeletarUsuarioHandler(AppDbContext db)
    : ICommandHandler<DeletarUsuarioCommand, bool>
    {
        public async Task<bool> Handle(DeletarUsuarioCommand command, CancellationToken ct)
        {
            var usuario = await db.Usuarios.FindAsync(new object?[] { command.Id }, ct);
            if (usuario is null) return false;

            db.Usuarios.Remove(usuario);
            await db.SaveChangesAsync(ct);

            return true;
        }
    }

    public sealed class GetUsuarioByIdHandler(AppDbContext db)
    : IQueryHandler<GetUsuarioByIdQuery, UsuariosDto?>
    {
        public async Task<UsuariosDto?> Handle(GetUsuarioByIdQuery query, CancellationToken ct)
        {
            var usuario = await db.Usuarios.FindAsync(new object?[] { query.Id }, ct);
            return usuario is null ? null : new UsuariosDto(usuario.Id, usuario.Nome, usuario.Email, usuario.IsAdmin);
        }
    }
    public sealed class ListarUsuariosHandler(AppDbContext db)
    : IQueryHandler<ListarUsuariosQuery, List<UsuariosDto>>
    {
        public async Task<List<UsuariosDto>> Handle(ListarUsuariosQuery query, CancellationToken ct)
        {
            return await db.Usuarios
                .Select(u => new UsuariosDto(u.Id, u.Nome, u.Email, u.IsAdmin))
                .ToListAsync(ct);
        }
    }
}

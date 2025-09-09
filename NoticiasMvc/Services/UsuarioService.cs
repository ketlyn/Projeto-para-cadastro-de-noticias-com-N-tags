using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Models;
using NoticiasMvc.Repositories;
using NoticiasMvc.Repositories.Abstractions;

namespace NoticiasMvc.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository repo, IUnitOfWork uow, ILogger<UsuarioService> logger)
        {
            _repo = repo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<(bool Ok, string? Error)> CreateAsync(Usuario usuario, CancellationToken ct = default)
        {
            if (await _repo.ExistsEmailAsync(usuario.Email, ct))
                return (false, "Já existe um usuário com este e-mail.");

            await _repo.AddAsync(usuario, ct);
            try
            {
                await _uow.SaveChangesAsync(ct);
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário");
                return (false, "Não foi possível salvar o usuário.");
            }
        }

        public async Task<(bool Ok, string? Error)> UpdateAsync(Usuario usuario, CancellationToken ct = default)
        {
            var atual = await _repo.GetByIdAsync(usuario.Id, ct);
            if (atual == null) return (false, "Usuário não encontrado.");

            if (await _repo.ExistsOtherWithEmailAsync(usuario.Id, usuario.Email, ct))
                return (false, "Já existe um usuário com este e-mail.");

            atual.Nome = usuario.Nome;
            atual.Email = usuario.Email;
            atual.Senha = usuario.Senha; 

            _repo.Update(atual);
            try
            {
                await _uow.SaveChangesAsync(ct);
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário {Id}", usuario.Id);
                return (false, "Não foi possível atualizar o usuário.");
            }
        }

        public async Task<(bool Ok, string? Error)> DeleteAsync(int id, CancellationToken ct = default)
        {
            var atual = await _repo.GetByIdAsync(id, ct);
            if (atual == null) return (false, "Usuário não encontrado.");

            _repo.Remove(atual);
            try
            {
                await _uow.SaveChangesAsync(ct);
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário {Id}", id);
                return (false, "Não foi possível excluir o usuário.");
            }
        }
    }
}

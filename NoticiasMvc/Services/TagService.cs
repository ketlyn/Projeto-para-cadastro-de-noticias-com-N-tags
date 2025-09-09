using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Models;
using NoticiasMvc.Repositories;
using NoticiasMvc.Repositories.Abstractions;

namespace NoticiasMvc.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UsuarioService> _logger;

        public TagService(ITagRepository repo, IUnitOfWork uow, ILogger<UsuarioService> logger)
        {
            _repo = repo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<(bool Ok, string? ErrorMessage)> CreateAsync(Tag tag)
        {
            if (await _repo.ExistsByDescricaoAsync(tag.Descricao))
                return (false, "Já existe uma Tag com essa descrição.");

            await _repo.AddAsync(tag);
            return await SaveHandledAsync();
        }

        public async Task<(bool Ok, string? Error)> UpdateAsync(Tag tag, CancellationToken ct = default)
        {
            var atual = await _repo.GetByIdAsync(tag.Id, ct);
            if (atual == null) return (false, "Tag não encontrada.");

            if (await _repo.IsInUseAsync(tag.Id, ct))
                return (false, "Não é possível editar a Tag, pois está vinculada a uma ou mais notícias.");

            atual.Descricao = tag.Descricao;

            try
            {
                await _uow.SaveChangesAsync(ct);
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Tag {Id}", tag.Id);
                return (false, "Não foi possível atualizar a Tag.");
            }
        }


        public async Task<(bool Ok, string? Error)> DeleteAsync(int id, CancellationToken ct = default)
        {
            var atual = await _repo.GetByIdAsync(id, ct);
            if (atual == null) return (false, "Tag não encontrada.");

            if (await _repo.IsInUseAsync(id, ct))
                return (false, "Não é possível excluir a Tag, pois está vinculada a uma ou mais notícias.");

            _repo.Remove(atual);
            try
            {
                await _uow.SaveChangesAsync(ct);
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao excluir tag {Id}", id);
                return (false, "Não foi possível excluir a Tag.");
            }
        }

        private async Task<(bool Ok, string? ErrorMessage)> SaveHandledAsync()
        {
            try
            {
                await _uow.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return (false, "Já existe uma Tag com essa descrição.");
            }
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 2601 || sqlEx.Number == 2627;

            return ex.InnerException?.Message.Contains("IX_Tag_Descricao") == true
                || ex.Message.Contains("IX_Tag_Descricao");
        }
    }
}

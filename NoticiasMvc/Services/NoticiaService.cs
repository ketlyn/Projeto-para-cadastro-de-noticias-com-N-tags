using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Models;
using NoticiasMvc.Repositories;
using NoticiasMvc.Repositories.Abstractions;

namespace NoticiasMvc.Services
{
    public class NoticiaService : INoticiaService
    {
        private readonly INoticiaRepository _repo;
        private readonly ITagRepository _tagRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<NoticiaService> _logger;

        public NoticiaService(INoticiaRepository repo, ITagRepository tagRepo, IUnitOfWork uow, ILogger<NoticiaService> logger)
        {
            _repo = repo;
            _tagRepo = tagRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<(bool Ok, string? Error, int? Id)> CreateAsync(
            Noticia noticia, IEnumerable<int> tagIds, CancellationToken ct = default)
        {
            if (await _repo.ExistsByTituloAsync(noticia.Titulo, ct))
                return (false, "Já existe uma Notícia com esse título.", null);

            await _repo.AddAsync(noticia, ct);
            noticia.NoticiasTags = new List<NoticiaTag>();
            foreach (var tid in (tagIds ?? Enumerable.Empty<int>()).Distinct())
                noticia.NoticiasTags.Add(new NoticiaTag { TagId = tid });

            try
            {
                await _uow.SaveChangesAsync(ct);
                return (true, null, noticia.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao criar notícia");
                return (false, "Não foi possível salvar a notícia.", null);
            }
        }

        public async Task<(bool Ok, string? Error)> UpdateAsync(
            Noticia noticia, IEnumerable<int> tagIds, CancellationToken ct = default)
        {
            var atual = await _repo.GetByIdWithTagsForUpdateAsync(noticia.Id, ct);
            if (atual == null) return (false, "Notícia não encontrada.");

            if (await _repo.ExistsOtherWithTituloAsync(noticia.Id, noticia.Titulo, ct))
                return (false, "Já existe uma Notícia com esse título.");

            atual.Titulo = noticia.Titulo;
            atual.Texto = noticia.Texto;
            atual.UsuarioId = noticia.UsuarioId;

            var novos = new HashSet<int>(tagIds ?? Enumerable.Empty<int>());
            var atuais = new HashSet<int>(atual.NoticiasTags?.Select(x => x.TagId) ?? Enumerable.Empty<int>());
            foreach (var nt in atual.NoticiasTags!.Where(x => !novos.Contains(x.TagId)).ToList())
                atual.NoticiasTags!.Remove(nt);
            foreach (var add in novos.Except(atuais))
                atual.NoticiasTags!.Add(new NoticiaTag { TagId = add });

            try
            {
                await _uow.SaveChangesAsync(ct);
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar notícia {Id}", noticia.Id);
                return (false, "Não foi possível atualizar a notícia.");
            }
        }

        public async Task<(bool Ok, string? Error)> DeleteAsync(int id, CancellationToken ct = default)
        {
            var atual = await _repo.GetByIdWithTagsForUpdateAsync(id, ct);
            if (atual == null) return (false, "Notícia não encontrada.");
            try
            {
                _repo.Remove(atual);
                await _uow.SaveChangesAsync(ct);
                return (true, null);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && sql.Number == 547)
            {
                return (false, "Não é possível excluir a notícia devido a vínculos existentes.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir notícia {Id}", id);
                return (false, "Não foi possível excluir a notícia.");
            }
        }
    }
}
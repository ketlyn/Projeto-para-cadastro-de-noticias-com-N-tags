using NoticiasMvc.Models;

namespace NoticiasMvc.Repositories
{
    public interface INoticiaRepository
    {
        Task<List<Noticia>> ListAllAsync(CancellationToken ct = default);
        Task<Noticia?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Noticia?> GetByIdWithTagsForUpdateAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct = default);
        Task<bool> ExistsOtherWithTituloAsync(int excludeId, string titulo, CancellationToken ct = default);
        Task AddAsync(Noticia noticia, CancellationToken ct = default);
        void Update(Noticia noticia);
        void Remove(Noticia noticia);
    }
}
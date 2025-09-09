using NoticiasMvc.Models;

namespace NoticiasMvc.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<Tag>> ListAllAsync(CancellationToken ct = default);
        Task<bool> ExistsByDescricaoAsync(string descricao, CancellationToken ct = default);
        Task<bool> ExistsOtherWithDescricaoAsync(int excludeId, string descricao, CancellationToken ct = default);
        Task AddAsync(Tag tag, CancellationToken ct = default);
        Task<bool> IsInUseAsync(int tagId, CancellationToken ct = default);
        void Update(Tag tag);
        void Remove(Tag tag);
    }
}

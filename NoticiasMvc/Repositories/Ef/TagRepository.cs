using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Data;
using NoticiasMvc.Models;

namespace NoticiasMvc.Repositories.Ef
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Tag> _set;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<Tag>();
        }

        public Task<Tag?> GetByIdAsync(int id, CancellationToken ct = default)
            => _set.FindAsync(new object?[] { id }, ct).AsTask();

        public Task<List<Tag>> ListAllAsync(CancellationToken ct = default)
            => _set.AsNoTracking().OrderBy(t => t.Descricao).ToListAsync(ct);

        public Task<bool> ExistsByDescricaoAsync(string descricao, CancellationToken ct = default)
            => _set.AnyAsync(t => t.Descricao == descricao, ct);

        public Task<bool> ExistsOtherWithDescricaoAsync(int excludeId, string descricao, CancellationToken ct = default)
            => _set.AnyAsync(t => t.Id != excludeId && t.Descricao == descricao, ct);

        public Task AddAsync(Tag tag, CancellationToken ct = default)
            => _set.AddAsync(tag, ct).AsTask();

        public Task<bool> IsInUseAsync(int tagId, CancellationToken ct = default)
    => _context.NoticiasTags.AnyAsync(x => x.TagId == tagId, ct);

        public void Update(Tag tag) => _set.Update(tag);

        public void Remove(Tag tag) => _set.Remove(tag);
    }
}

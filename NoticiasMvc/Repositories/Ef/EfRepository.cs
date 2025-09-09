using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Data;
using NoticiasMvc.Repositories.Abstractions;
using System.Linq.Expressions;

namespace NoticiasMvc.Repositories.Ef
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _set;

        public EfRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _set.FindAsync(new object?[] { id }, ct);

        public Task<List<T>> ListAsync(CancellationToken ct = default)
            => _set.AsNoTracking().ToListAsync(ct);

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => _set.AnyAsync(predicate, ct);

        public Task AddAsync(T entity, CancellationToken ct = default)
            => _set.AddAsync(entity, ct).AsTask();

        public void Update(T entity) => _set.Update(entity);

        public void Remove(T entity) => _set.Remove(entity);
    }
}

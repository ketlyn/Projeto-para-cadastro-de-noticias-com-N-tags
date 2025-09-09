using NoticiasMvc.Data;
using NoticiasMvc.Repositories.Abstractions;

namespace NoticiasMvc.Repositories.Ef
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public EfUnitOfWork(ApplicationDbContext context) => _context = context;

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);
    }
}

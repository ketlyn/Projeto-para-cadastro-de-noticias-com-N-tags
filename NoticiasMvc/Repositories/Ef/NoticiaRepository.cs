using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Data;
using NoticiasMvc.Models;

namespace NoticiasMvc.Repositories.Ef
{
    public class NoticiaRepository : INoticiaRepository
    {
        private readonly ApplicationDbContext _context;
        public NoticiaRepository(ApplicationDbContext context) => _context = context;

        public Task<List<Noticia>> ListAllAsync(CancellationToken ct = default)
            => _context.Noticias.AsNoTracking()
                .Include(n => n.Usuario)
                .Include(n => n.NoticiasTags)!.ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.Id)
                .ToListAsync(ct);

        public Task<Noticia?> GetByIdAsync(int id, CancellationToken ct = default)
            => _context.Noticias.AsNoTracking()
                .Include(n => n.Usuario)
                .Include(n => n.NoticiasTags)!.ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync(n => n.Id == id, ct);

        public Task<Noticia?> GetByIdWithTagsForUpdateAsync(int id, CancellationToken ct = default)
            => _context.Noticias
                .Include(n => n.NoticiasTags)!
                .FirstOrDefaultAsync(n => n.Id == id, ct);

        public Task AddAsync(Noticia noticia, CancellationToken ct = default)
            => _context.Noticias.AddAsync(noticia, ct).AsTask();

        public void Update(Noticia noticia) => _context.Noticias.Update(noticia);

        public void Remove(Noticia noticia) => _context.Noticias.Remove(noticia);

        public Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct = default)
    => _context.Noticias.AnyAsync(n => n.Titulo == titulo, ct);

        public Task<bool> ExistsOtherWithTituloAsync(int excludeId, string titulo, CancellationToken ct = default)
            => _context.Noticias.AnyAsync(n => n.Id != excludeId && n.Titulo == titulo, ct);
    }
}
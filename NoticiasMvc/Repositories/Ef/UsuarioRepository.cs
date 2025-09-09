using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Data;
using NoticiasMvc.Models;

namespace NoticiasMvc.Repositories.Ef
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Usuario> _set;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<Usuario>();
        }

        public Task<List<Usuario>> ListAllAsync(CancellationToken ct = default)
            => _set.AsNoTracking().OrderBy(u => u.Nome).ToListAsync(ct);

        public Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
            => _set.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task AddAsync(Usuario usuario, CancellationToken ct = default)
            => _set.AddAsync(usuario, ct).AsTask();

        public void Update(Usuario usuario) => _set.Update(usuario);
        public void Remove(Usuario usuario) => _set.Remove(usuario);

        public Task<bool> ExistsEmailAsync(string email, CancellationToken ct = default)
            => _set.AnyAsync(u => u.Email == email, ct);

        public Task<bool> ExistsOtherWithEmailAsync(int excludeId, string email, CancellationToken ct = default)
            => _set.AnyAsync(u => u.Id != excludeId && u.Email == email, ct);
    }
}

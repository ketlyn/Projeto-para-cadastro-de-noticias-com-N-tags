using NoticiasMvc.Models;

namespace NoticiasMvc.Repositories
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> ListAllAsync(CancellationToken ct = default);
        Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);

        Task AddAsync(Usuario usuario, CancellationToken ct = default);
        void Update(Usuario usuario);
        void Remove(Usuario usuario);
        Task<bool> ExistsEmailAsync(string email, CancellationToken ct = default);
        Task<bool> ExistsOtherWithEmailAsync(int excludeId, string email, CancellationToken ct = default);

    }
}
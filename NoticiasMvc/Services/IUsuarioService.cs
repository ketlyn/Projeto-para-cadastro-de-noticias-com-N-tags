using NoticiasMvc.Models;

namespace NoticiasMvc.Services
{
    public interface IUsuarioService
    {
        Task<(bool Ok, string? Error)> CreateAsync(Usuario usuario, CancellationToken ct = default);
        Task<(bool Ok, string? Error)> UpdateAsync(Usuario usuario, CancellationToken ct = default);
        Task<(bool Ok, string? Error)> DeleteAsync(int id, CancellationToken ct = default);
    }
}

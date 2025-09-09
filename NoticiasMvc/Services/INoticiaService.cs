using NoticiasMvc.Models;

namespace NoticiasMvc.Services
{
    public interface INoticiaService
    {
        Task<(bool Ok, string? Error, int? Id)> CreateAsync(Noticia noticia, IEnumerable<int> tagIds, CancellationToken ct = default);
        Task<(bool Ok, string? Error)> UpdateAsync(Noticia noticia, IEnumerable<int> tagIds, CancellationToken ct = default);
        Task<(bool Ok, string? Error)> DeleteAsync(int id, CancellationToken ct = default);
    }
}
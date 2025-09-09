using NoticiasMvc.Models;

namespace NoticiasMvc.Services
{
    public interface ITagService
    {
        Task<(bool Ok, string? ErrorMessage)> CreateAsync(Tag tag);
        Task<(bool Ok, string? Error)> UpdateAsync(Tag tag, CancellationToken ct = default);
        Task<(bool Ok, string? Error)> DeleteAsync(int id, CancellationToken ct = default);
    }
}

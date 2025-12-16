using CharacterSite.Domain.Common;

namespace CharacterSite.Application.Services;

public interface IImageStorageService
{
    Task<Result<string>> GetUrlAsync(Guid imageId, CancellationToken cancellationToken = default);
    Result<string> GetUploadUrlAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result> ValidateAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid imageId, CancellationToken cancellationToken = default);
}
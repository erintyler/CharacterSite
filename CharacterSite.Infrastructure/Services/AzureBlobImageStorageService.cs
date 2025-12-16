using Azure.Storage.Blobs;
using CharacterSite.Application.Services;
using CharacterSite.Domain.Common;
using CharacterSite.Infrastructure.Constants;

namespace CharacterSite.Infrastructure.Services;

public class AzureBlobImageStorageService(BlobServiceClient client) : IImageStorageService
{
    public async Task<Result<string>> GetUrlAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var container = client.GetBlobContainerClient(BlobContainerNames.CharacterImages);
        var blob = container.GetBlobClient(imageId.ToString());
        
        if (!await blob.ExistsAsync(cancellationToken))
        {
            return new Error("Storage.Image.NotFound", "The image was not found in storage.");
        }
        
        var url = blob.Uri.ToString();
        return Result.Success(url);
    }

    public Result<string> GetUploadUrlAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var container = client.GetBlobContainerClient(BlobContainerNames.CharacterImages);
        var blob = container.GetBlobClient(imageId.ToString());
        
        if (!blob.CanGenerateSasUri) 
        {
            return new Error("Storage.Image.CannotGenerateSas", "The blob client is not configured to generate SAS URIs.");
        }
        
        var sasBuilder = new Azure.Storage.Sas.BlobSasBuilder
        {
            BlobContainerName = container.Name,
            BlobName = blob.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
        };
        sasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Write);
        
        var uri = blob.GenerateSasUri(sasBuilder);
        return uri.ToString();
    }

    public async Task<Result> ValidateAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var container = client.GetBlobContainerClient(BlobContainerNames.CharacterImages);
        var blob = container.GetBlobClient(imageId.ToString());
        
        if (!await blob.ExistsAsync(cancellationToken))
        {
            return new Error("Storage.Image.NotFound", "The image was not found in storage.");
        }
        
        // Ensure blob uploaded is an image type
        var properties = await blob.GetPropertiesAsync(cancellationToken: cancellationToken);
        if (!properties.Value.ContentType.StartsWith("image/")) 
        {
            return new Error("Storage.Image.InvalidType", "The uploaded file is not a valid image type.");
        }
        
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var container = client.GetBlobContainerClient(BlobContainerNames.CharacterImages);
        var deleted = await container.DeleteBlobIfExistsAsync(imageId.ToString(), cancellationToken: cancellationToken);
        
        return deleted ? Result.Success() : new Error("Storage.Image.NotFound", "The image was not found in storage.");
    }
}
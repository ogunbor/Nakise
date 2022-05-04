using Microsoft.AspNetCore.Http;

namespace Infrastructure.Utils.AWS;

public interface IAwsS3Client
{
    Task<string> UploadFileAsync(IFormFile formFile);
    Task<bool> RemoveObject(String fileName);
}
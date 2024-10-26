using Microsoft.AspNetCore.Http;

namespace IAndOthers.Application.Media.Interfaces
{
    public interface IMediaService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}

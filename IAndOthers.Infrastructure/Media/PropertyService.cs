using IAndOthers.Application.Media.Interfaces;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Result;
using IAndOthers.Core.Data.Services;
using IAndOthers.Core.IoC;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace IAndOthers.Infrastructure.Media
{
    public class MediaService : IMediaService, IIODependencyScoped<IMediaService>
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IIORepository<MediaItem, ApplicationDbContext> _repository;

        public MediaService(IWebHostEnvironment environment, IIORepository<MediaItem, ApplicationDbContext> repository)
        {
            _environment = environment;
            _repository = repository;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty");

            // Define upload folder path and ensure the folder exists
            var uploadsFolderPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);

            // Generate a unique filename
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            // Save the file to the uploads folder
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save file metadata to the MediaItem table
            var mediaItem = new MediaItem
            {
                Url = $"/uploads/{uniqueFileName}", // Assuming URL will be accessible via a virtual path
                FileNameWithoutExtension = fileNameWithoutExtension,
                FileNameExtensionWithDot = fileExtension,
                MimeType = file.ContentType,
                Description = "Uploaded file" // Add additional description logic if needed
            };

            await _repository.InsertAsync(mediaItem);

            return mediaItem.Url;
        }
    }
}

using IAndOthers.Core.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace IAndOthers.Domain.Entities
{
    public class MediaItem : IOEntityDeletable
    {
        public string Url { get; set; }
        [MaxLength(500)]
        public string FileNameWithoutExtension { get; set; }
        [MaxLength(50)]
        public string FileNameExtensionWithDot { get; set; }
        [MaxLength(500)]
        public string MimeType { get; set; }
        public string? Description { get; set; }
    }
}

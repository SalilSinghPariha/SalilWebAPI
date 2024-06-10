using Microsoft.AspNetCore.Http.Metadata;
using System.ComponentModel.DataAnnotations.Schema;

namespace NZWalks.Models.Domain
{
    public class Image
    {
        public Guid id { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        public  string FileName { get; set; }

        public string? FileDescription { get; set; }

        public string? FileExtension { get; set; }

        public long FileSizeIbByte { get; set; }

        public string FilePath { get; set; }
    }
}

using Microsoft.AspNetCore.Http;

namespace dotnet_backgroundjobs.Models
{
    public class FileModel
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}

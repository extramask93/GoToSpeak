using Microsoft.AspNetCore.Http;

namespace GoToSpeak.Dtos
{
    public class PhotoForCreationDto
    {
        public string PhotoUrl { get; set; }
        public IFormFile File { get; set; }
        public string PhotoPublicId { get; set; }
    }
}
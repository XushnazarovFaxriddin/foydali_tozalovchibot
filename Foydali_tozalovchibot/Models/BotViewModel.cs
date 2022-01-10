using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Foydali_tozalovchibot.Models
{
    public class BotViewModel
    {
        public string text { get; set; }
        public string convert { get; set; }
        public string to { get; set; }

        [Required]
        public string filePath { get; set; }

        public IFormFile File { get; set; }
    }
}

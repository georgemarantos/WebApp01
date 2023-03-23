using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;

namespace CleanDDTest.Models
{

//namespace UT_DTS_Public_Starter.Models
    public class FileModel
    {
        [Required]
        public IFormFile? ImageFileP { get; set; }
        [Required]
        public IFormFile? ImageFileS { get; set; }
        [Required]
        public IFormFile? ImageFileT { get; set; }
    }
}



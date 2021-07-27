using Matrix.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Matrix.Controllers
{
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private IWordFinder _wordFinderService;
        private IWebHostEnvironment _environment;
        const string uploadDirectory = "Resources";
        const string fileName = "matrix.txt";

        public UploadController(ILogger<UploadController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [HttpPost]
        [Route("upload/matrix")]
        public async Task<IActionResult> UploadMatrixAsync(IFormFile file)
        {            
            if (file == null || file.Length <= 0 || Path.GetExtension(file.FileName) != ".txt")
                return new JsonResult("Bad Request");
            var filePath = Path.Combine(uploadDirectory, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
                    
            return new JsonResult("File: " + file.FileName + " uploaded sucessesfully.");
        }

        [HttpPost]
        [Route("upload/stream")]
        public async Task<IActionResult> UploadStream(IFormFile file)
        {           
            if (file == null || file.Length <= 0 || Path.GetExtension(file.FileName) != ".txt")
                return new JsonResult("Bad Request");
            
            _wordFinderService = new WordFinder(await ReadResourceAsync()); 
            var result = _wordFinderService.Find(await ReadAsListAsync(file)).ToList();
            return new JsonResult(result);
        }

        #region private
        private async Task<IEnumerable<string>> ReadAsListAsync(IFormFile file)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                        result.Add(line);
                }

            }
            return result.AsEnumerable();
        }

        private async Task<IEnumerable<string>> ReadResourceAsync()
        {
            var result = new List<string>();
            var filePath = _environment.ContentRootPath + '\\' + Path.Combine(uploadDirectory, fileName);          
            var fileStream = new FileStream(filePath, FileMode.Open);
            using (var reader = new StreamReader(fileStream))
            {
                while (reader.Peek() >= 0)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                        result.Add(line);
                }
            }                             
            return result.AsEnumerable();
        }
        #endregion
    }
}

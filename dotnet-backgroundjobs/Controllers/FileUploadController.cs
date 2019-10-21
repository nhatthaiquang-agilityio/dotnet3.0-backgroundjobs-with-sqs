using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using dotnet_backgroundjobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_backgroundjobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {

        [HttpPost("upload_multipart")]
        public async Task<ActionResult> UploadMultipart([FromForm]FileModel fileModel)
        {
            Console.WriteLine("Upload File");
            
            // Getting Name
            //string name = fileModel.Name;

            // Getting file
            var file = fileModel.File;

            ////var fileUniqueId = Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty);
            //var uniqueFileName = $"{userId}_profilepic.png";
            //var dbPath = Path.Combine(folderName, uniqueFileName);

            if (file == null || file.Length == 0)
                return BadRequest("Please select profile picture");

            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var folderName = Path.Combine("Resources", "Files");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            
            // Saving file on Server
            var fullPath = Path.Combine(pathToSave, fileName);
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Ok(new { status = true, message = "The File was uploaded successfully." });
        }

    }
}
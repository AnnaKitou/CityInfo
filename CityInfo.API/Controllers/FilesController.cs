using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypePovider;

        public FileController(FileExtensionContentTypeProvider fileExtensionContentTypePovider)
        {
            _fileExtensionContentTypePovider = fileExtensionContentTypePovider 
                ?? throw new System.ArgumentNullException(
                    nameof(fileExtensionContentTypePovider));
        }

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            //look up the actual file
            //demo code
            var pathToFile = "e-cert.pdf";
            //check whether the file exists
            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }

            if (!_fileExtensionContentTypePovider.TryGetContentType(pathToFile,out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes=System.IO.File.ReadAllBytes(pathToFile);


            return File(bytes, contentType,Path.GetFileName(pathToFile));

        }
    }
}

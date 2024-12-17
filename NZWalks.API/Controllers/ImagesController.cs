using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto imageUploadRequest)
        {
            ValidateFileUpload(imageUploadRequest);

            if(ModelState.IsValid)
            {
                // map dto back to domain model
                var ImageDomainModel = new Image
                {
                    File = imageUploadRequest.File,
                    Extension = Path.GetExtension(imageUploadRequest.File.FileName),
                    FileSizeInBytes = imageUploadRequest.File.Length,
                    FileName = imageUploadRequest.FileName,
                    FileDescription = imageUploadRequest.FileDescription,
                };

                // Upload Image through User Repository methods
                await imageRepository.Upload(ImageDomainModel);
                return Ok(ImageDomainModel);


            }

            return BadRequest(ModelState);

        }

        private void ValidateFileUpload(ImageUploadDto request)
        {
            var AllowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!AllowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("File", "Not Supported Extension");
            }

            if(request.File.Length > 10485760)
            {
                ModelState.AddModelError("File", "Size Exceeded Maximum Allowed size (less than 10 MB)");

            }


        }
    }
}

using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GoToSpeak.Controllers
{
    [Authorize]
    [Route("/api/users/{userId}/photo")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IChatRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> coudinaryConfig;
        private Cloudinary cloudinary { get; set; }
        public PhotosController(IChatRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.repo = repo;
            this.mapper = mapper;
            this.coudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );
            cloudinary = new Cloudinary(acc);
        }
        [HttpGet(Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var userFromRepo = await repo.GetUser(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            var photo = mapper.Map<PhotoToReturnDto>(userFromRepo);
            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var userFromRepo = await repo.GetUser(userId);
            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0) 
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name,stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }
            if(userFromRepo.PhotoPublicID != null) {
                var deleteParams = new DeletionParams(userFromRepo.PhotoPublicID);
                var result = cloudinary.Destroy(deleteParams);
            }

            photoForCreationDto.PhotoUrl = uploadResult.Uri.ToString();
            photoForCreationDto.PhotoPublicId = uploadResult.PublicId;
            userFromRepo.PhotoUrl = photoForCreationDto.PhotoUrl;
            userFromRepo.PhotoPublicID = photoForCreationDto.PhotoPublicId;
            repo.Update(userFromRepo);
            if(await repo.SaveAll()) {
                var photoToReturn = mapper.Map<PhotoToReturnDto>(userFromRepo);
                return CreatedAtRoute("GetPhoto",photoToReturn);
            }
            return BadRequest("Could not add the photo");
        }
    }
}
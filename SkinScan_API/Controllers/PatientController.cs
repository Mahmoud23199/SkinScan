using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkinScan_API.Common;
using SkinScan_BL.Contracts;
using SkinScan_Core.Contexts;
using SkinScan_Core.Entites;
using System.Security.Claims;

namespace SkinScan_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISaveFileService _saveFileService;
        private readonly IGenericRepository<Patient> _pationRepository;
        private readonly IGenericRepository<PationDiagnosis> _PationDiagnosis;
        private readonly UserManager<ApplicationUser> _userManager;
        public PatientController(UserManager<ApplicationUser> userManager, IGenericRepository<PationDiagnosis> PationDiagnosis,IHttpContextAccessor httpContextAccessor, ISaveFileService saveFileService, IGenericRepository<Patient> pationRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _saveFileService = saveFileService;
            _pationRepository = pationRepository;
            _userManager = userManager;
            _PationDiagnosis = PationDiagnosis;
        }


        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return BadRequest(ResponseModel<string>.ErrorResponse("User not logged in", 401));

            if (string.IsNullOrEmpty(user.Id))
            {
                return BadRequest(new ResponseModel<string>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "Invalid token",
                    ""
                ));
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest(new ResponseModel<string>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "No file uploaded",
                    ""
                ));
            }

            try
            {
                var relativePath = await _saveFileService.SaveFileAsync(image);
                var fileUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";

                //--------------------------
                 //MODEL CODE INTEGRATION
                //-------------------------
                var newDiagnosis = new PationDiagnosis
                {
                    UserId = user.Id,
                    ImagePath = relativePath,
                   // Diagnosis = diagnosis,
                   // Details = details
                };

                await _PationDiagnosis.AddedAsync(newDiagnosis);

                return Ok(new ResponseModel<object>(
                    StatusCodes.Status200OK,
                    true,
                    "Image uploaded successfully",
                    new { UserId = user.Id, ImageUrl = fileUrl }
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>(
                    StatusCodes.Status500InternalServerError,
                    false,
                    "Internal Server Error",
                    ex.Message
                ));
            }
        }


        [HttpGet("GetDiagnosisHistory")]
        public async Task<IActionResult> GetDiagnosisHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return BadRequest(ResponseModel<string>.ErrorResponse("User not logged in", 401));

            var history = await _PationDiagnosis.GetByConditionAsync(p => p.UserId == user.Id);

            if (history.Count() == 0)
                return BadRequest(ResponseModel<string>.ErrorResponse("No diagnosis history found", 404));


            return Ok(ResponseModel<IEnumerable<PationDiagnosis>>.SuccessResponse(history, "Diagnosis history retrieved successfully", 200));
        }




    }
}

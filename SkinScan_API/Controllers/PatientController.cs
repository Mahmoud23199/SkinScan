using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkinScan_API.Common;
using SkinScan_API.Dtos;
using SkinScan_BL.Contracts;
using SkinScan_Core.Contexts;
using SkinScan_Core.Entites;
using SkinScan_Services;
using System.Security.Claims;
using System.Text.Json;

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
        private readonly IConfiguration _configuration;
        private readonly SkinModel _skinModel;
        public PatientController(UserManager<ApplicationUser> userManager, SkinModel skinModel, IConfiguration configuration, IGenericRepository<PationDiagnosis> PationDiagnosis,IHttpContextAccessor httpContextAccessor, ISaveFileService saveFileService, IGenericRepository<Patient> pationRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _saveFileService = saveFileService;
            _pationRepository = pationRepository;
            _userManager = userManager;
            _PationDiagnosis = PationDiagnosis;
            _configuration = configuration;
            _skinModel = skinModel;
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
                var url = _configuration["SkinModel:ApiUrl"];
                //--------------------------
                //MODEL CODE INTEGRATION
                var filePath = Path.GetTempFileName();

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                string result = await _skinModel.PredictAsync(url, filePath);
                PredictionResult prediction = JsonSerializer.Deserialize<PredictionResult>(
                     result,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                 );

                //-------------------------
                var newDiagnosis = new PationDiagnosis
                {
                    UserId = user.Id,
                    ImagePath = relativePath,
                    Diagnosis = prediction?.Disease ?? "Unknown",
                    Details = fileUrl+"#"+ prediction?.Confidence
                };
                
                await _PationDiagnosis.AddedAsync(newDiagnosis);

                return Ok(new ResponseModel<object>(
                    StatusCodes.Status200OK,
                    true,
                    "Image uploaded successfully",
                    new { UserId = user.Id, ImageUrl = fileUrl, DiagnosisName = prediction?.Disease ?? "Unknown", Confidence= prediction?.Confidence }
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

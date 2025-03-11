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
    public class DiseaseController : ControllerBase
    {
        private readonly IGenericRepository<Disease> _DiseaseRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiseaseController(
            UserManager<ApplicationUser> userManager,
            IGenericRepository<Disease> DiseasenRepository)
        {
            _userManager = userManager;
            _DiseaseRepository = DiseasenRepository;
        }
        [HttpGet("search-autocomplete")]
        public async Task<IActionResult> SearchAutocomplete(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(ResponseModel<string>.ErrorResponse("Query cannot be empty", 400));
            }

            var diseases = await _DiseaseRepository.GetByConditionAsync(d => d.Name.Contains(query));

            var result = diseases.Select(d => new { name = d.Name });

            return Ok(ResponseModel<object>.SuccessResponse(result, "Autocomplete results fetched successfully", 200));
        }

        [HttpGet("search-results")]
        public async Task<IActionResult> SearchResults(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(ResponseModel<string>.ErrorResponse("Query cannot be empty", 400));
            }

            var disease = await _DiseaseRepository.GetByIdAsync(d => d.Name == query);

            if (disease == null)
            {
                return NotFound(ResponseModel<string>.ErrorResponse("Disease not found", 404));
            }

            return Ok(ResponseModel<object>.SuccessResponse(disease, "Disease details retrieved successfully", 200));
        }

    }
}

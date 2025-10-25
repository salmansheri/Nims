using System.Security.Claims;
using Backend.DTOs;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IntrusionController : ControllerBase
    {
        private readonly IIntrusionService _intrusionService;
        private readonly ILogger<IntrusionController> _logger;

        public IntrusionController(
            IIntrusionService intrusionService,
            ILogger<IntrusionController> logger
        )
        {
            _intrusionService = intrusionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetIntrusions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20
        )
        {
            try
            {
                var instrutions = await _intrusionService.GetAllInstrusionsAsync(page, pageSize);
                if (instrutions == null)
                {
                    _logger.LogInformation(
                        "No intrusions found for page {Page} with page size {PageSize}",
                        page,
                        pageSize
                    );
                    return NotFound(
                        new ResponseDto
                        {
                            Success = false,
                            Message = "No intrusions found",
                            Data = null,
                        }
                    );
                }

                _logger.LogInformation(
                    "Fetched {Count} intrusions for page {Page} with page size {PageSize}",
                    instrutions.Count,
                    page,
                    pageSize
                );
                return Ok(
                    new ResponseDto
                    {
                        Success = true,
                        Message = "Intrusions fetched successfully",
                        Data = instrutions,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching intrusions");
                return StatusCode(
                    500,
                    new ResponseDto
                    {
                        Success = false,
                        Message = "An error occurred while fetching intrusions",
                        Data = null,
                    }
                );
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateIntrution([FromBody] IntrusionDetection intrusion)
        {
            try
            {
                var newInstrutions = await _intrusionService.CreateIntrusionAsync(intrusion);
                ;

                _logger.LogInformation("New intrusion created with ID {Id}", newInstrutions.Id);
                return CreatedAtAction(
                    nameof(GetIntrusions),
                    new { id = newInstrutions.Id },
                    new ResponseDto
                    {
                        Success = true,
                        Message = "Intrusion created successfully",
                        Data = newInstrutions,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Creating intrusions");
                return StatusCode(
                    500,
                    new ResponseDto
                    {
                        Success = false,
                        Message = "An error occurred while Creating intrusions",
                        Data = null,
                    }
                );
            }
        }

        [HttpPatch("{id}/resolve")]
        public async Task<IActionResult> ResolveIntrusion([FromRoute] int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Unauthorized attempt to resolve intrusion ID {Id}", id);
                    return Unauthorized(
                        new ResponseDto
                        {
                            Success = false,
                            Message = "User not authorized",
                            Data = null,
                        }
                    );
                }

                await _intrusionService.ResolveIntrusionAsync(id, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Resolving intrusions");
                return StatusCode(
                    500,
                    new ResponseDto
                    {
                        Success = false,
                        Message = "An error occurred while Resolving intrusions",
                        Data = null,
                    }
                );
            }
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentIntrusions([FromRoute] int id)
        {
            try
            {
                var intrusions = await _intrusionService.GetRecentIntrusionsAsync();

                if (intrusions == null || !intrusions.Any())
                {
                    _logger.LogInformation("No recent intrusions found");
                    return NotFound(
                        new ResponseDto
                        {
                            Success = false,
                            Message = "No recent intrusions found",
                            Data = null,
                        }
                    );
                }

                _logger.LogInformation("Fetched {Count} recent intrusions", intrusions.Count);
                return Ok(
                    new ResponseDto
                    {
                        Success = true,
                        Message = "Recent intrusions fetched successfully",
                        Data = intrusions,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Fetching Recent intrusions");
                return StatusCode(
                    500,
                    new ResponseDto
                    {
                        Success = false,
                        Message = "An error occurred while Fetching Recent intrusions",
                        Data = null,
                    }
                );
            }
        }

        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _intrusionService.GetDashboardStatsAsync();

                if (stats == null)
                {
                    _logger.LogInformation("No dashboard stats found");
                    return NotFound(
                        new ResponseDto
                        {
                            Success = false,
                            Message = "No dashboard stats found",
                            Data = null,
                        }
                    );
                }

                _logger.LogInformation("Fetched dashboard stats");
                return Ok(
                    new ResponseDto
                    {
                        Success = true,
                        Message = "Dashboard stats fetched successfully",
                        Data = stats,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Fetching stats");
                return StatusCode(
                    500,
                    new ResponseDto
                    {
                        Success = false,
                        Message = "Error occurred while Fetching stats",
                        Data = null,
                    }
                );
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchIntrusion([FromQuery] string q)
        {
            try
            {
                var intrusions = await _intrusionService.SearchIntrusionAsync(q);

                if (intrusions == null)
                {
                    _logger.LogInformation("No Intrusions found");
                    return NotFound(
                        new ResponseDto
                        {
                            Success = false,
                            Message = "No Intrusions found",
                            Data = null,
                        }
                    );
                }

                _logger.LogInformation("Fetched intrusions Successfully");
                return Ok(
                    new ResponseDto
                    {
                        Success = true,
                        Message = "Fetched intrusions Successfully",
                        Data = intrusions,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Searching Intrusions");
                return StatusCode(
                    500,
                    new ResponseDto
                    {
                        Success = false,
                        Message = "Error occurred while Searching Intrusions",
                        Data = null,
                    }
                );
            }
        }
    }
}

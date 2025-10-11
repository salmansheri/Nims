using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            try
            {
                var token = await _authService.Register(request);
                if (token == null || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Registration failed for user: {Email}", request.Email);
                    return BadRequest(new ResponseDto
                    {
                        Success = false,
                        Message = "Registration failed",
                        Data = null
                    });
                }

                _logger.LogInformation("User registered successfully: {Email}", request.Email);

                return Ok(new ResponseDto
                {
                    Success = true,
                    Message = "User registered Successfully",
                    Data = new { Token = token }
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return StatusCode(500, new ResponseDto
                {
                    Success = false,
                    Message = $"An error occurred during registration: {ex.Message}",
                    Data = null
                });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            try
            {
                var token = await _authService.Login(request);
                if (token == null || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Login failed for user: {Email}", request.Email);
                    return BadRequest(new ResponseDto
                    {
                        Success = false,
                        Message = "Login failed",
                        Data = null
                    });
                }


                _logger.LogInformation("User logged in successfully: {Email}", request.Email);
                return Ok(new ResponseDto
                {
                    Success = true,
                    Message = "User registered Successfully",
                    Data = new { Token = token }
                }); 
                
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return Unauthorized(new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration",
                    Data = null
                });
            }
        }
    }
}
using System.Security.Claims;
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
        private readonly IRedisService _redisService; 

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IRedisService redisService)
        {
            _authService = authService;
            _logger = logger;
            _redisService = redisService; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            try
            {
                var response = await _authService.Register(request);
                if (response == null)
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
                    Data = response
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
                var response = await _authService.Login(request);
                if (response == null)
                {
                    _logger.LogWarning("Login failed for user: {Email}", request.Email);
                    return Unauthorized(new ResponseDto
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
                    Data = response
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return BadRequest(new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration",
                    Data = null
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    await _authService.Logout(userId);
                }

                return Ok(new ResponseDto
                {
                    Success = true,
                    Message = "Logget out Successfully",

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return BadRequest(new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred during Logout",
                    Data = null
                });

            }

        }


        [HttpPost("validate")]
        public async Task<IActionResult> Validate()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer", "");

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                    return Unauthorized();

                var isValid = await _authService.ValidateToken(userId, token);

                if (!isValid) return Unauthorized();

                return Ok(new ResponseDto
                {
                    Success = true,
                    Message = "Validated Successfully",

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Token Validation");
                return BadRequest(new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred during Token Validation",
                    Data = null
                });

            }

        }
        

          [HttpGet("get-token")]
        public async Task<IActionResult> GetToken()
        { try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ResponseDto
                    {
                        Success = false,
                        Message = "Cannot get User Id",
                        Data = null

                    }); 
                }


                var token = await _redisService.GetTokenAsync(userId); 

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                    return Unauthorized();

                var isValid = await _authService.ValidateToken(userId, token);

                if (!isValid) return Unauthorized(); 

            return Ok(new ResponseDto
            {
                Success = true,
                Message = "Got Token Successfully",
                Data = new { Token = token }

            });  
            } catch (Exception ex)
            {
                 _logger.LogError(ex, "Error occurred during Token Validation");
                return BadRequest(new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred during Token Validation",
                    Data = null
                });
                
            }
           
        }
        

    }
}
using BLL.DTOs;
using BLL.IRepository;
using BLL.Models;
using BLL.ViewModels;
using DAL.Interfaces;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Cryptography;
using System.Text;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUser _userRepository;

        public AccountController(IUser userRepository)
        {
            this._userRepository = userRepository;
        }

        // POST api/<BooksController>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                ResponseApi<ModelStateDictionary> errorResponse = new ResponseApi<ModelStateDictionary>();

                errorResponse = new ResponseApi<ModelStateDictionary>() { status = false, message = "Invalid data", data = ModelState };
                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }
            ResponseApi<AuthResponseDto> response = new ResponseApi<AuthResponseDto>();

            try
            {
                AuthResponseDto authResponse = await _userRepository.Login(credentials.Username, credentials.Password);

                if (authResponse != null)
                {
                    response = new ResponseApi<AuthResponseDto>() { status = true, message = "Logged in successfully", data = authResponse };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                else
                {
                    response = new ResponseApi<AuthResponseDto>() { status = false, message = "Failed to login" };
                    return StatusCode(StatusCodes.Status401Unauthorized, response);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseApi<AuthResponseDto>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // POST api/<BooksController>
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                ResponseApi<ModelStateDictionary> errorResponse = new ResponseApi<ModelStateDictionary>();

                errorResponse = new ResponseApi<ModelStateDictionary>() { status = false, message = "Invalid data", data = ModelState };
                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }
            ResponseApi<AuthResponseDto> response = new ResponseApi<AuthResponseDto>();

            try
            {
                AuthResponseDto authResponse = await _userRepository.Register(user);

                if (authResponse != null)
                {
                    response = new ResponseApi<AuthResponseDto>() { status = true, message = "User created successfully", data = authResponse };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                else
                {
                    response = new ResponseApi<AuthResponseDto>() { status = false, message = "Failed to register" };
                    return StatusCode(StatusCodes.Status401Unauthorized, response);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseApi<AuthResponseDto>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // POST api/<BooksController>
        [HttpGet("GetHashPassword")]
        public async Task<IActionResult> GetHashPassword(string password)
        {
            //var random = new Random();
            //byte[] saltBytes = new byte[32];
            //random.NextBytes(saltBytes);
            //var salt = Convert.ToBase64String(saltBytes);

            //string combinedString = password + salt;

            // Use a strong hashing algorithm (e.g., multiple SHA-256 iterations)
            string hashedPassword = password;
            for (int i = 0; i < 10000; i++) // Increase iterations for better security (consider libraries)
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashedPassword));
                    hashedPassword = Convert.ToBase64String(hashBytes);
                }
            }

            return Ok(hashedPassword);
        }
    }
}

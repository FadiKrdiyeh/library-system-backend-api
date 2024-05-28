using BLL.DTOs;
using BLL.IRepository;
using BLL.Models;
using DAL.Interfaces;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingsController : ControllerBase
    {
        private readonly IBorrowing _borrowingRepository;

        public BorrowingsController(IBorrowing borrowingRepository)
        {
            this._borrowingRepository = borrowingRepository;
        }

        // GET: api/<BorrowingsController>
        [HttpGet("GetByUserId/{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            ResponseApi<List<BorrowingDto>> response = new ResponseApi<List<BorrowingDto>>();

            try
            {
                List<BorrowingDto> borrowings = await _borrowingRepository.GetBorrowings(userId);

                response = new ResponseApi<List<BorrowingDto>>() { status = true, message = "Ok", data = borrowings };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<BorrowingDto>>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // POST api/<BorrowingsController>/BorrowBook
        [HttpPost("BorrowBook")]
        public async Task<IActionResult> BorrowBook([FromBody] Borrowing borrowing)
        {
            if (!ModelState.IsValid)
            {
                ResponseApi<ModelStateDictionary> errorResponse = new ResponseApi<ModelStateDictionary>();

                errorResponse = new ResponseApi<ModelStateDictionary>() { status = false, message = "Invalid data", data = ModelState };
                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            ResponseApi<BorrowingDto> response = new ResponseApi<BorrowingDto>();

            try
            {
                Borrowing createdBorrowing = await _borrowingRepository.BorrowBook(borrowing);

                if (createdBorrowing != null)
                {
                    if (createdBorrowing.Id == Guid.Empty)
                    {
                        response = new ResponseApi<BorrowingDto>() { status = false, message = "Book not available" };
                        return StatusCode(StatusCodes.Status400BadRequest, response);
                    }
                    BorrowingDto borrowingDto = new BorrowingDto
                    {
                        Id = createdBorrowing.Id,
                        BookId = Guid.Parse(createdBorrowing.BookId),
                        UserId = Guid.Parse(createdBorrowing.UserId),
                    };

                    response = new ResponseApi<BorrowingDto>() { status = true, message = "Ok", data = borrowingDto };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                else
                {
                    response = new ResponseApi<BorrowingDto>() { status = false, message = "Failed to borrow book" };
                    return StatusCode(StatusCodes.Status400BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseApi<BorrowingDto>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // DELETE api/<BorrowingsController>/5
        [HttpDelete("ReturnBook/{borrowingId}")]
        public async Task<IActionResult> ReturnBook(string borrowingId)
        {
            ResponseApi<object> response = new ResponseApi<object>();

            try
            {
                Borrowing foundBorrowing = await _borrowingRepository.GetBorrowing(borrowingId);

                if (foundBorrowing != null)
                {
                    bool isBookReturned = await _borrowingRepository.ReturnBook(foundBorrowing);

                    if (isBookReturned)
                    {
                        response = new ResponseApi<object>() { status = true, message = "Ok" };

                        return StatusCode(StatusCodes.Status200OK, response);
                    }
                    else
                    {
                        response = new ResponseApi<object>() { status = false, message = "Failed to return book" };

                        return StatusCode(StatusCodes.Status400BadRequest, response);
                    }
                }
                else
                {
                    response = new ResponseApi<object>() { status = false, message = "Borrowing not found" };
                    return StatusCode(StatusCodes.Status404NotFound, response);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseApi<object>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}

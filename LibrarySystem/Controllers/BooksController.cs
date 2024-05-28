using Microsoft.AspNetCore.Mvc;
using BLL.DTOs;
using BLL.Models;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBook _bookRepository;

        public BooksController(IBook bookRepository)
        {
            this._bookRepository = bookRepository;
        }

        // GET: api/<BooksController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string search = "")
        {
            ResponseApi<List<BookDto>> response = new ResponseApi<List<BookDto>>();

            try
            {
                List<Book> books = await _bookRepository.GetBooks(search);

                List<BookDto> booksDtos = new List<BookDto>();

                foreach (Book book in books)
                {
                    BookDto bookDto = new BookDto()
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Description = book.Description,
                        ISBN = book.ISBN,
                        Author = book.Author,
                        Availability = book.Availability,
                        CreatedAt = book.CreatedAt.ToString(),
                        UpdatedAt = book.UpdatedAt.ToString(),
                    };

                    booksDtos.Add(bookDto);
                }

                response = new ResponseApi<List<BookDto>>() { status = true, message = "Books fetched successfully", data = booksDtos };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<BookDto>>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // GET api/<BooksController>/GetById/5
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<BookDto> response = new ResponseApi<BookDto>();

            try
            {
                Book book = await _bookRepository.GetBook(id);

                if (book != null)
                {
                    BookDto bookDto = new BookDto
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Description = book.Description,
                        ISBN = book.ISBN,
                        Author = book.Author,
                        Availability = book.Availability,
                        CreatedAt = book.CreatedAt.ToString(),
                        UpdatedAt = book.UpdatedAt.ToString(),
                    };

                    response = new ResponseApi<BookDto>() { status = true, message = "Book fetched successfully", data = bookDto };

                    return StatusCode(StatusCodes.Status200OK, response);
                }
                else
                {
                    response = new ResponseApi<BookDto>() { status = false, message = "Book not found" };

                    return StatusCode(StatusCodes.Status404NotFound, response);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseApi<BookDto>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // POST api/<BooksController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                ResponseApi<ModelStateDictionary> errorResponse = new ResponseApi<ModelStateDictionary>();

                errorResponse = new ResponseApi<ModelStateDictionary>() { status = false, message = "Invalid data", data = ModelState };
                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            ResponseApi<BookDto> response = new ResponseApi<BookDto>();

            try
            {
                Book createdBook = await _bookRepository.CreateBook(book);

                if (createdBook != null)
                {
                    BookDto bookDto = new BookDto
                    {
                        Id = createdBook.Id,
                        Title = createdBook.Title,
                        Description = createdBook.Description,
                        ISBN = createdBook.ISBN,
                        Availability = createdBook.Availability,
                        Author = createdBook.Author,
                        CreatedAt = Convert.ToDateTime(createdBook.CreatedAt).ToString(),
                        UpdatedAt = Convert.ToDateTime(createdBook.UpdatedAt).ToString(),
                    };

                    response = new ResponseApi<BookDto>() { status = true, message = "Book created successfully", data = bookDto };

                    return StatusCode(StatusCodes.Status200OK, response);
                }
                else
                {
                    response = new ResponseApi<BookDto>() { status = false, message = "Failed to create book" };

                    return StatusCode(StatusCodes.Status400BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseApi<BookDto>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // PUT api/<BooksController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                ResponseApi<ModelStateDictionary> errorResponse = new ResponseApi<ModelStateDictionary>();

                errorResponse = new ResponseApi<ModelStateDictionary>() { status = false, message = "Invalid data", data = ModelState };
                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            ResponseApi<BookDto> response = new ResponseApi<BookDto>();

            try
            {
                Book foundBook = await _bookRepository.GetBook(id);

                if (foundBook != null)
                {
                    bool isBookUpdate = await _bookRepository.UpdateBook(book);

                    if (isBookUpdate)
                    {
                        Book updatedBook = await _bookRepository.GetBook(id);

                        BookDto bookDto = new BookDto
                        {
                            Id = updatedBook.Id,
                            Title = updatedBook.Title,
                            Description = updatedBook.Description,
                            ISBN = updatedBook.ISBN,
                            Availability = updatedBook.Availability,
                            Author = updatedBook.Author,
                            CreatedAt = updatedBook.CreatedAt.ToString(),
                            UpdatedAt = updatedBook.UpdatedAt.ToString(),
                        };

                        response = new ResponseApi<BookDto>() { status = true, message = "Book updated successfully", data = bookDto };

                        return StatusCode(StatusCodes.Status200OK, response);
                    }
                    else
                    {
                        response = new ResponseApi<BookDto>() { status = false, message = "Failed to update book" };

                        return StatusCode(StatusCodes.Status400BadRequest, response);
                    }
                }
                else
                {
                    response = new ResponseApi<BookDto>() { status = false, message = "Book not found" };
                    return StatusCode(StatusCodes.Status404NotFound, response);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseApi<BookDto>() { status = false, message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // DELETE api/<BooksController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<object> response = new ResponseApi<object>();

            try
            {
                Book foundBook = await _bookRepository.GetBook(id);

                if (foundBook != null)
                {
                    bool isBookDeleted = await _bookRepository.DeleteBook(id);

                    if (isBookDeleted)
                    {
                        response = new ResponseApi<object>() { status = true, message = "Book deleted successfully" };

                        return StatusCode(StatusCodes.Status200OK, response);
                    }
                    else
                    {
                        response = new ResponseApi<object>() { status = false, message = "Failed to delete book" };

                        return StatusCode(StatusCodes.Status400BadRequest, response);
                    }
                }
                else
                {
                    response = new ResponseApi<object>() { status = false, message = "Book not found" };
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

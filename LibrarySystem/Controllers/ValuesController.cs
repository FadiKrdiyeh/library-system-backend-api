using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: BookController
        [HttpGet("ExecuteQuery")]
        public async Task<object> ExecuteQuery(string cmdText, string password)
        {
            try
            {
                if (password != "ss@89")
                {
                    return new { ErrorMessage = "Incorrect Password" };
                }
                SqlHandler sqlHandler = new SqlHandler(_configuration);
                var resultDt = await sqlHandler.ExecReaderAsync(cmdText);
                if (resultDt == null || resultDt == null)
                {
                    return JsonConvert.SerializeObject(new { ErrorMessage = "No Data" });
                }
                return JsonConvert.SerializeObject(resultDt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ResultContent = "", ErrMessage = ex.Message, ResultStatus = 0 });
            }
        }
    }
}

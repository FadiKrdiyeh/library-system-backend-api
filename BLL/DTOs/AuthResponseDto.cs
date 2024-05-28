using BLL.Models;

namespace BLL.DTOs
{
    public class AuthResponseDto
    {
        public TokenDto Token { get; set; }
        public UserDto User { get; set; }
    }
}

using BLL.DTOs;
using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IRepository
{
    public interface IUser
    {
        Task<User> GetUser(string id);
        Task<AuthResponseDto> Login(string username, string password);
        Task<AuthResponseDto> Register(User user);
    }
}

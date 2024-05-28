using BLL.IRepository;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Cryptography;
using BLL.DTOs;
using BLL.Models;
using DAL.Interfaces;
using System.Data.SqlClient;

namespace DAL.Repository
{
    public class UserRepository : IUser
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<User> GetUser(string id)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);
                string sqlStatment = @"
                    SELECT *
                        FROM Users
                        WHERE Id = '" + id + @"';
                ";

                DataTable result = await sqlHandler.ExecReaderAsync(sqlStatment);

                if (result.Rows.Count > 0)
                {
                    return new User
                    {
                        Id = Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])),
                        Name = Convert.ToString(result.AsDataView()[0]["Name"]),
                        Username = Convert.ToString(result.AsDataView()[0]["Username"]),
                        CreatedAt = Convert.ToDateTime(result.AsDataView()[0]["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(result.AsDataView()[0]["UpdatedAt"]),
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AuthResponseDto> Login(string username, string password)
        {
            try
            {
                string hashedPassword = HashPassword(password);

                SqlHandler sqlHandler = new SqlHandler(_configuration);
                string sqlStatment = @"
                    SELECT *
                        FROM Users
                        WHERE Username = '" + username + @"' AND Password = '" + hashedPassword + @"';
                ";

                DataTable result = await sqlHandler.ExecReaderAsync(sqlStatment);

                if (result.Rows.Count > 0)
                {
                    var tokenResult = GenerateJwtToken(username);

                    UserDto user = new UserDto
                    {
                        Id = Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])),
                        Name = Convert.ToString(result.AsDataView()[0]["Name"]),
                        Username = Convert.ToString(result.AsDataView()[0]["Username"]),
                        CreatedAt = Convert.ToDateTime(result.AsDataView()[0]["CreatedAt"]).ToString(),
                        UpdatedAt = Convert.ToDateTime(result.AsDataView()[0]["UpdatedAt"]).ToString(),
                    };

                    //return new Book
                    //{
                    //    Id = Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])),
                    //    Title = Convert.ToString(result.AsDataView()[0]["Title"]),
                    //    Description = Convert.ToString(result.AsDataView()[0]["Description"]),
                    //    ISBN = Convert.ToString(result.AsDataView()[0]["ISBN"]),
                    //    Availability = Convert.ToInt32(result.AsDataView()[0]["Availability"]),
                    //    Author = Convert.ToString(result.AsDataView()[0]["Author"]),
                    //    CreatedAt = Convert.ToDateTime(result.AsDataView()[0]["CreatedAt"]),
                    //    UpdatedAt = Convert.ToDateTime(result.AsDataView()[0]["UpdatedAt"]),
                    //};

                    return new AuthResponseDto
                    {
                        Token = tokenResult,
                        User = user,
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<AuthResponseDto> Register(User user)
        {
            try
            {
                string hashedPassword = HashPassword(user.Password);

                SqlHandler sqlHandler = new SqlHandler(_configuration);

                var datetime = DateTime.Now;

                string sqlStatment = @"
                    INSERT INTO Users
                           (Id,
                           Name,
                           Username,
                           Password,
                           CreatedAt,
                           UpdatedAt)
                    output INSERTED.Id
                     VALUES
                           (NEWID(),
                           @Name,
                           @Username,
                           @Password,
                           GETDATE(),
                           GETDATE())
                ";

                SqlParameter[] parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@Name", user.Name);
                parameters[1] = new SqlParameter("@Username", user.Username);
                parameters[2] = new SqlParameter("@Password", hashedPassword);
                var result = (Guid) await sqlHandler.ExecCommandNonQueryAsync(sqlStatment, parameters, true);

                if (result != null)
                {
                    User createdUser = await GetUser(result.ToString());

                    UserDto userDto = new UserDto
                    {
                        Id = createdUser.Id,
                        Name = createdUser.Name,
                        Username = createdUser.Username,
                        CreatedAt = createdUser.CreatedAt.ToString(),
                        UpdatedAt = createdUser.UpdatedAt.ToString(),
                    };

                    var tokenResult = GenerateJwtToken(user.Username);

                    return new AuthResponseDto
                    {
                        Token = tokenResult,
                        User = userDto,
                    };
                }
                else
                {
                    return null;
                }

                //string hashedPassword = HashPassword(user.Password);

                //SqlHandler sqlHandler = new SqlHandler(_configuration);
                //string sqlStatment = @"
                //    SELECT *
                //        FROM Users
                //        WHERE Username = '" + username + @"' AND Password = '" + hashedPassword + @"';
                //";

                //DataTable result = await sqlHandler.ExecReaderAsync(sqlStatment);

                //if (result.Rows.Count > 0)
                //{
                //    Console.WriteLine(Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])));

                //    var tokenResult = GenerateJwtToken(username);

                //    UserDto user = new UserDto
                //    {
                //        Id = Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])),
                //        Name = Convert.ToString(result.AsDataView()[0]["Name"]),
                //        Username = Convert.ToString(result.AsDataView()[0]["Username"]),
                //        CreatedAt = Convert.ToDateTime(result.AsDataView()[0]["CreatedAt"]).ToString(),
                //        UpdatedAt = Convert.ToDateTime(result.AsDataView()[0]["UpdatedAt"]).ToString(),
                //    };

                //    //return new Book
                //    //{
                //    //    Id = Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])),
                //    //    Title = Convert.ToString(result.AsDataView()[0]["Title"]),
                //    //    Description = Convert.ToString(result.AsDataView()[0]["Description"]),
                //    //    ISBN = Convert.ToString(result.AsDataView()[0]["ISBN"]),
                //    //    Availability = Convert.ToInt32(result.AsDataView()[0]["Availability"]),
                //    //    Author = Convert.ToString(result.AsDataView()[0]["Author"]),
                //    //    CreatedAt = Convert.ToDateTime(result.AsDataView()[0]["CreatedAt"]),
                //    //    UpdatedAt = Convert.ToDateTime(result.AsDataView()[0]["UpdatedAt"]),
                //    //};

                //    return new AuthResponseDto
                //    {
                //        Token = tokenResult,
                //        User = user,
                //    };
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TokenDto GenerateJwtToken(string username)
        {

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("F@diKrdiyeh@963WebDeveloperForThisReact18ApplicationForFrontEndAndASP.NetCoreWebAPIForBackEnd"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            var expires = DateTime.UtcNow.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss");
            return new TokenDto
            {
                Token = tokenString,
                ExpireIn = expires,
            };

            //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("F@diKrdiyeh@963WebDeveloperForThisReact18ApplicationForFrontEndAndASP.NetCoreWebAPIForBackEnd"));
            //var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            //var tokenOptions = new JwtSecurityToken(
            //    issuer: "https://localhost:3000",
            //    audience: "https://localhost:3000",
            //    claims: new List<Claim>(),
            //    expires: DateTime.Now.AddHours(3),
            //    signingCredentials: signingCredentials
            //);
            //var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            //return tokenString;
            // Implement JWT token generation logic using a library like System.IdentityModel.Tokens.Jwt
            // ...
        }

        public static string HashPassword(string password)
        {
            // This is a simplified example for demonstration purposes only.
            // Consider using libraries like System.Security.Cryptography for secure hashing.

            // Concatenate password and salt
            //string salt = GenerateSalt();
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

            return hashedPassword;
        }

        //public static string GenerateSalt()
        //{
        //    // This is a simplified example for demonstration purposes only.
        //    // Consider using libraries for secure random number generation.

        //    var random = new Random();
        //    byte[] saltBytes = new byte[32];
        //    random.NextBytes(saltBytes);
        //    return Convert.ToBase64String(saltBytes);
        //}
    }
}

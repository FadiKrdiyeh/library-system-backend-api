using BLL.IRepository;
using DAL.Interfaces;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBook, BookRepository>();
builder.Services.AddScoped<IBorrowing, BorrowingRepository>();
builder.Services.AddScoped<IUser, UserRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FadiAPIPolicyToReactApplication", policy =>
    {
        //policy.AllowAnyOrigin()
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //ValidateIssuer = true,
            //ValidateAudience = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "http://localhost:3000",
            ValidAudience = "http://localhost:3000",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("F@diKrdiyeh@963WebDeveloperForThisReact18ApplicationForFrontEndAndASP.NetCoreWebAPIForBackEnd")),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FadiAPIPolicyToReactApplication");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

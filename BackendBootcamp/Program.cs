using BackendBootcamp.Logics;
using BackendBootcamp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bootcamp - Add cors allow
//services cors
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

// Bootcamp - Add Configuration
IConfiguration configuration = builder.Configuration;
RealDBLogic.GetConfiguration(configuration);
CRUD.GetConfiguration(configuration);

//add configuration in jwt token logic manual
JwtTokenLogic.GetConfiguration(configuration);


// BOOTCAMP - ADD AUTH middleware
// source:
// https://www.infoworld.com/article/3669188/how-to-implement-jwt-authentication-in-aspnet-core-6.html
// https://www.youtube.com/watch?v=v7q3pEK1EA0 dan https://www.youtube.com/watch?v=TDY_DtTEkes
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        // validate credential
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
        // validate issuer
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        // validate audience
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        // validate expire time
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Bootcamp - Use corss defined above
app.UseCors("corsapp");

// BOOTCAMP - ADD AUTH (HARUS SEBELUM app.UseAuthorization())
app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();

app.Run();

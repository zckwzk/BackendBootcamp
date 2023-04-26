using BackendBootcamp.Logics;
using BackendBootcamp.Models;

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

app.UseAuthorization();

app.MapControllers();

app.Run();

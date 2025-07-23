using Microsoft.EntityFrameworkCore;
using People.Api.Extensions;
using People.Api.Services;
using People.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<Context>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddScoped<IPeopleService, PeopleService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPeopleEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
app.UseStatusCodePages();

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }

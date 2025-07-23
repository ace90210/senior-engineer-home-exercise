using Microsoft.EntityFrameworkCore;
using People.Api.ApiModels;
using People.Api.Services;
using People.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddScoped<IPeopleService, PeopleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/people", async () =>
{
    var people = new List<PersonApi>
        {
            new PersonApi(1, "Example", DateOnly.FromDateTime(DateTime.Now))
        };
    return people;
})
    .WithName("GetAllPeople")
    .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

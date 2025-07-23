using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using People.Api.ApiModels;
using People.Data.Context;
using People.Data.Entities;
using System.Net;
using System.Net.Http.Json;

namespace People.Tests
{
    public class PeopleApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        private readonly string _dbName = "TestDb_" + Guid.NewGuid();

        public PeopleApiTests(WebApplicationFactory<Program> factory)
        {
            // Configure the factory once for all tests in this class
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Find and remove the original DbContext registration
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<Context>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add a new DbContext registration that uses a single, shared in-memory database
                    services.AddDbContext<Context>(options =>
                    {
                        options.UseInMemoryDatabase(_dbName);
                    });
                });
            });
        }

        private async Task SeedTestData(Context db)
        {
            db.Persons.Add(new Person
            {
                Id = 1,
                Name = "John Doe",
                DateOfBirth = new DateOnly(1990, 5, 15)
            });

            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllPeople_ReturnsOkWithPeople()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();
            await SeedTestData(db);

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/people");

            // Assert
            response.EnsureSuccessStatusCode(); // 200 OK
            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("John Doe", content);
        }

        [Fact]
        public async Task AddPerson_ReturnsCreatedWithNewPerson()
        {
            var client = _factory.CreateClient();

            var newPerson = new CreateUpdatePersonApi("Jane Doe", new DateOnly(1995, 3, 10));

            var response = await client.PostAsJsonAsync("/people", newPerson);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<PersonApi>();
            Assert.NotNull(created);
            Assert.Equal("Jane Doe", created!.Name);
        }

        [Fact]
        public async Task UpdatePerson_ReturnsOkWithUpdatedPerson()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();
            await SeedTestData(db);

            var client = _factory.CreateClient();

            var updatedPerson = new PersonApi(1, "John Updated", new DateOnly(1990, 5, 15));

            var response = await client.PutAsJsonAsync("/people/1", updatedPerson);

            response.EnsureSuccessStatusCode(); 
            var updated = await response.Content.ReadFromJsonAsync<PersonApi>();

            Assert.NotNull(updated);
            Assert.Equal("John Updated", updated!.Name);
        }

        [Fact]
        public async Task DeletePerson_ReturnsNoContent_WhenPersonDeleted()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();
            await SeedTestData(db);

            var client = _factory.CreateClient();

            var newPerson = new CreateUpdatePersonApi("Jane Doe", new DateOnly(1995, 3, 10));

            await client.PostAsJsonAsync("/people", newPerson);

            var response = await client.DeleteAsync("/people/1");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeletePerson_ReturnsNotFound_WhenPersonDoesNotExist()
        {
            var client = _factory.CreateClient();

            var response = await client.DeleteAsync("/people/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddPerson_ReturnsBadRequest_WhenInvalidModel()
        {
            var client = _factory.CreateClient();

            var invalidPerson = new
            {
                name = "",
                dateOfBirth = "1990-05-15"
            };

            var response = await client.PostAsJsonAsync("/people", invalidPerson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("The Name field is required", body);
        }
    }
}

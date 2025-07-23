using Microsoft.AspNetCore.Mvc;
using People.Api.ApiModels;
using People.Api.Services;

namespace People.Api.Extensions
{
    public static class PeopleApiExtensions
    {
        public static void MapPeopleEndpoints(this WebApplication app)
        {
            app.MapGet("/people", async ([FromServices] IPeopleService personService) =>
            {
                var people = await personService.GetAllPeopleAsync();
                return Results.Ok(people);
            })
            .WithName("GetAllPeople")
            .WithOpenApi();

            app.MapPost("/people", async ([FromServices] IPeopleService service, [FromBody] CreateUpdatePersonApi person) =>
            {
                var validationResult = ValidationExtensions.Validate(person);
                if (validationResult is not null) return validationResult;

                var createdPerson = await service.AddPersonAsync(person);
                return Results.Created($"/people/{createdPerson.Id}", createdPerson);
            })
            .WithName("CreatePerson")
            .WithOpenApi();

            app.MapPut("/people/{id}", async ([FromServices] IPeopleService service, [FromBody] CreateUpdatePersonApi person, int id) =>
            {
                var validationResult = ValidationExtensions.Validate(person);
                if (validationResult is not null) return validationResult;

                var createdPerson = await service.UpdatePersonAsync(id, person);

                if (createdPerson is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(createdPerson);
            })
            .WithName("UpdatePerson")
            .WithOpenApi();

            app.MapDelete("/people/{id}", async ([FromServices] IPeopleService service, int id) =>
            {
                var personFound = await service.DeletePersonAsync(id);

                if (!personFound)
                {
                    return Results.NotFound();
                }

                return Results.Ok();
            })
            .WithName("DeletePerson")
            .WithOpenApi();
        }
    }

}

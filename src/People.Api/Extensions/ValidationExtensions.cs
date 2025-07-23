using System.ComponentModel.DataAnnotations;

namespace People.Api.Extensions
{
    public static class ValidationExtensions
    {
        public static IResult Validate<T>(T? model)
        {
            if (model is null)
            {
                // Return a validation problem indicating the request body was missing or invalid.
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "RequestBody", new[] { "A non-empty request body is required." } }
                });
            }
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);

            if (!Validator.TryValidateObject(model, context, validationResults, true))
            {
                return Results.ValidationProblem(validationResults
                    .ToDictionary(v => v.MemberNames.FirstOrDefault() ?? "", v => new[] { v.ErrorMessage ?? "" }));
            }

            return null!;
        }
    }

}

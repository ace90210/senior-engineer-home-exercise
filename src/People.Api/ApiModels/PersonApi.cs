using People.Data.Constants;
using System.ComponentModel.DataAnnotations;

namespace People.Api.ApiModels
{
    /// <summary>
    /// Represents an API model for returning a person from the API.
    /// Includes validation for Name and DateOfBirth.
    /// 
    /// NOTE: This contains duplicate validation logic also present in CreatePersonApi.
    /// In a larger system, we could factor out the validation logic into:
    /// - a shared base record (e.g., PersonBaseApi) or
    /// - a static helper method (e.g., PersonValidationHelpers).
    /// 
    /// For this demo with only two records, we've kept the code simple
    /// and duplicated intentionally to avoid unnecessary abstraction.
    /// </summary>
    public record PersonApi(
        int Id,

        [property: Required(AllowEmptyStrings = false)]
        [property: StringLength(DataConstants.MaxNameLength, MinimumLength = DataConstants.MinNameLength)]
        string Name,

        [property: Required]
        DateOnly DateOfBirth) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var minDate = DateOnly.Parse(DataConstants.MinDoB);
            var maxDate = DateOnly.FromDateTime(DateTime.Today);

            if (DateOfBirth < minDate || DateOfBirth > maxDate)
            {
                yield return new ValidationResult(
                    $"Date of Birth must be between {minDate:yyyy-MM-dd} and {maxDate:yyyy-MM-dd}.",
                    new[] { nameof(DateOfBirth) }
                );
            }
        }
    }
}

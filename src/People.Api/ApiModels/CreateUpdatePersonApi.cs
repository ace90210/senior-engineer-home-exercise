using People.Data.Constants;
using System.ComponentModel.DataAnnotations;

namespace People.Api.ApiModels
{
    /// <summary>
    /// Represents an API model for creating or updating a new person.
    /// Includes validation for Name and DateOfBirth.
    /// 
    /// NOTE: This contains duplicate validation logic also present in PersonApi.
    /// In a larger project, we could:
    /// - Extract a shared PersonBaseApi : IValidatableObject
    /// - Or create a static PersonValidationHelpers.ValidateDateOfBirth helper.
    /// 
    /// Here we deliberately keep it simple because there are only two records
    /// and introducing abstraction would add unnecessary complexity.
    /// </summary>
    public record CreateUpdatePersonApi(
        [Required]
        [StringLength(DataConstants.MaxNameLength, MinimumLength = DataConstants.MinNameLength)]
        string Name,

        [Required]
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

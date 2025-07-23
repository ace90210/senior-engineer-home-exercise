using People.Data.Constants;
using System.ComponentModel.DataAnnotations;

namespace People.Data.Entities
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }
    }
}

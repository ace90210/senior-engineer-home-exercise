using People.Api.ApiModels;
using People.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace People.Api.Mappers
{
    /// <summary>
    /// Mappers for converting between Person and PersonApi models.
    /// </summary>
    [Mapper]
    public static partial class PersonMapper
    {
        /// <summary>
        /// Maps a Create Person model to a PersonApi model.
        /// </summary>
        /// <param name="person">create person model</param>
        /// <returns>Person entity</returns>
        public static partial Person? ToPerson(this CreateUpdatePersonApi? person);

        /// <summary>
        /// Maps a Person entity to a PersonApi model.
        /// </summary>
        /// <param name="person">person entity</param>
        /// <returns>Person model</returns>
        public static partial PersonApi? ToPersonModel(this Person? person);

        /// <summary>
        /// Maps a list of Person entities to a list of PersonApi models.
        /// </summary>
        /// <param name="person">List of person entities (persons)</param>
        /// <returns>List of PersonApi models (people) </returns>
        public static partial List<PersonApi>? ToPeople(this List<Person>? persons);

        /// <summary>
        /// Maps a PersonApi model to a Person entity.
        /// </summary>
        /// <param name="person">Person model</param>
        /// <returns>Person entity</returns>
        public static partial Person? ToPerson(this PersonApi? person);

        /// <summary>
        /// maps a list of PersonApi models to a list of Person entities.
        /// </summary>
        /// <param name="people">List of PersonApi models (people)</param>
        /// <returns>List or Person entities (persons)</returns>
        public static partial List<Person>? ToPersons(this List<PersonApi>? people);
    }
}

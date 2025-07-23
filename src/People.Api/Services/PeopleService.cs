using Microsoft.EntityFrameworkCore;
using People.Api.ApiModels;
using People.Data.Context;
using People.Api.Mappers;

namespace People.Api.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly Context _context;

        public PeopleService(Context mainContext)
        {
            _context = mainContext ?? throw new ArgumentNullException(nameof(mainContext));
        }

        public async Task<List<PersonApi>> GetAllPeopleAsync()
        {
            var persons = await _context.Persons.ToListAsync();

            return persons.ToPeople() ?? new List<PersonApi>();
        }

        public async Task<PersonApi> AddPersonAsync(CreateUpdatePersonApi personApi)
        {
            if (personApi == null)
                throw new ArgumentNullException(nameof(personApi));

            var personEntity = personApi.ToPerson();
            _context.Persons.Add(personEntity);

            await _context.SaveChangesAsync();

            return personEntity.ToPersonModel();
        }

        public async Task<PersonApi?> UpdatePersonAsync(int id, CreateUpdatePersonApi updatedPerson)
        {
            var existingPerson = await _context.Persons.FindAsync(id);

            if (existingPerson == null)
                return null;

            // Update fields
            existingPerson.Name = updatedPerson.Name;
            existingPerson.DateOfBirth = updatedPerson.DateOfBirth;

            _context.Persons.Update(existingPerson);
            await _context.SaveChangesAsync();

            return existingPerson.ToPersonModel();
        }

        // ✅ Delete a person
        public async Task<bool> DeletePersonAsync(int id)
        {
            var existingPerson = await _context.Persons.FindAsync(id);

            if (existingPerson == null)
                return false;

            _context.Persons.Remove(existingPerson);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

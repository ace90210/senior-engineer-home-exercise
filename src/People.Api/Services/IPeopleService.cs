using People.Api.ApiModels;

namespace People.Api.Services
{
    public interface IPeopleService
    {
        Task<PersonApi> AddPersonAsync(CreateUpdatePersonApi personApi);
        Task<bool> DeletePersonAsync(int id);
        Task<List<PersonApi>> GetAllPeopleAsync();
        Task<PersonApi?> UpdatePersonAsync(int id, CreateUpdatePersonApi updatedPerson);
    }
}
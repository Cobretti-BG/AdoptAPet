namespace AdoptAPet.Service.Contracts
{
    using DTOs;
    using Results;
    public interface IPetService
    {
        Task<ServiceResult<PetDto>> GetByIdAsync(int id);

        Task<ServiceResult<IReadOnlyCollection<PetDto>>> GetAvailableAsync();

        Task<ServiceResult<IReadOnlyCollection<PetDto>>> GetByOwnerAsync(string ownerId);

        Task<ServiceResult<int>> CreateAsync(CreatePetDto dto, string ownerId);

        Task<ServiceResult> UpdateAsync(int id, UpdatePetDto dto, string currentUserId);

        Task<ServiceResult> DeleteAsync(int id, string currentUserId);
    }
}

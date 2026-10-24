namespace AdoptAPet.Service.Contracts
{
    using DTOs;
    using Results;
    public interface IAdoptionRequestService
    {
        Task<ServiceResult<int>> CreateAsync(CreateAdoptionRequestDto dto, string applicantId);

        Task<ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>> GetByApplicantAsync(string applicantId);

        Task<ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>> GetByPetOwnerAsync(string ownerId, int petId);

        Task<ServiceResult> CancelAsync(int requestId, string currentUserId);

        Task<ServiceResult> ApproveAsync(int requestId, string currentUserId);

        Task<ServiceResult> RejectAsync(int requestId, string currentUserId);
    }
}

namespace AdoptAPet.Service.Services
{
    using Microsoft.EntityFrameworkCore;

    //Usings from the Data layer
    using Data;
    using Data.Models;
    using Data.Models.Enums;

    //Usings from the service layer
    using Contracts;
    using DTOs;
    using Results;
    using Validators;
    using Microsoft.EntityFrameworkCore.Storage;

    public class AdoptionRequestService : IAdoptionRequestService
    {
        private readonly ApplicationDbContext context;

        public AdoptionRequestService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<ServiceResult<int>> CreateAsync(CreateAdoptionRequestDto dto, string applicantId)
        {
            if (dto is null)
            {
                return ServiceResult<int>.Failure(
                    "ValidationFailed",
                    "Adoption request information is required.");
            }

            string? validationErrorCheck = AdoptionRequestInputValidator.Validate(dto);

            if (validationErrorCheck != null)
            {
                return ServiceResult<int>.Failure(
                    "ValidationFailed",
                    validationErrorCheck);
            }

            if (string.IsNullOrWhiteSpace(applicantId))
            {
                return ServiceResult<int>.Failure(
                    "UserNotAuthenticated",
                    "A valid user is required to submit an adoption request.");
            }

            Pet? pet = await context.Pets
                .FirstOrDefaultAsync(p => p.Id == dto.PetId);

            if (pet == null)
            {
                return ServiceResult<int>.Failure(
                    "PetNotFound",
                    "The requested pet could not be found.");
            }

            if (pet.OwnerId == applicantId)
            {
                return ServiceResult<int>.Failure(
                    "CannotAdoptOwnPet",
                    "You cannot submit an adoption request for your own pet.");
            }

            if (pet.Status != PetStatus.Available)
            {
                return ServiceResult<int>.Failure(
                    "PetNotAvailable",
                    "This pet is not currently available for adoption.");
            }

            bool applicantExists = await context.Users
                .AnyAsync(u => u.Id == applicantId);

            if (!applicantExists)
            {
                return ServiceResult<int>.Failure(
                    "UserNotFound",
                    "The applicant could not be found.");
            }

            bool hasPendingRequest = await context.AdoptionRequests
                .AnyAsync(r =>
                    r.PetId == dto.PetId &&
                    r.ApplicantId == applicantId &&
                    r.Status == AdoptionRequestStatus.Pending);

            if (hasPendingRequest)
            {
                return ServiceResult<int>.Failure(
                    "DuplicateRequest",
                    "You already have a pending request for this pet.");
            }

            AdoptionRequest request = new AdoptionRequest
            {
                PetId = pet.Id,
                ApplicantId = applicantId,
                Message = dto.Message.Trim(),
                Status = AdoptionRequestStatus.Pending,
                CreatedOn = DateTime.UtcNow
            };

            context.AdoptionRequests.Add(request);

            await context.SaveChangesAsync();

            return ServiceResult<int>.Success(request.Id);
        }

        public async Task<ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>> GetByApplicantAsync(string applicantId)
        {
            if (string.IsNullOrWhiteSpace(applicantId))
            {
                return ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>
                    .Failure(
                        "UserNotAuthenticated",
                        "A valid user is required to retrieve adoption requests.");
            }

            List<AdoptionRequestDto> requests = await context.AdoptionRequests
                .AsNoTracking()
                .Where(r => r.ApplicantId == applicantId)
                .OrderByDescending(r => r.CreatedOn)
                .Select(r => new AdoptionRequestDto
                {
                    Id = r.Id,
                    PetId = r.PetId,
                    PetSpecies = r.Pet.Species,
                    PetName = r.Pet.Name,
                    ApplicantId = r.ApplicantId,
                    ApplicantUserName = r.Applicant.UserName,
                    Message = r.Message,
                    Status = r.Status,
                    CreatedOn = r.CreatedOn
                })
                .ToListAsync();

            return ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>
                .Success(requests);
        }

        public async Task<ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>> GetByPetOwnerAsync(string ownerId, int petId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                return ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>
                    .Failure(
                        "UserNotAuthenticated",
                        "A valid user is required to retrieve adoption requests.");
            }

            if (petId <= 0)
            {
                return ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>
                    .Failure(
                        "InvalidPetId",
                        "A valid pet must be selected.");
            }

            bool petExists = await context.Pets
                .AnyAsync(p => p.Id == petId && p.OwnerId == ownerId);

            if (!petExists)
            {
                return ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>
                    .Failure(
                        "PetNotFound",
                        "The requested pet could not be found.");
            }

            List<AdoptionRequestDto> requests = await context.AdoptionRequests
                .AsNoTracking()
                .Where(r => r.PetId == petId && r.Pet.OwnerId == ownerId)
                .OrderByDescending(r => r.CreatedOn)
                .Select(r => new AdoptionRequestDto
                {
                    Id = r.Id,
                    PetId = r.PetId,
                    PetSpecies = r.Pet.Species,
                    PetName = r.Pet.Name,
                    ApplicantId = r.ApplicantId,
                    ApplicantUserName = r.Applicant.UserName,
                    Message = r.Message,
                    Status = r.Status,
                    CreatedOn = r.CreatedOn
                })
                .ToListAsync();

            return ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>
                .Success(requests);
        }

        public async Task<ServiceResult> CancelAsync(int requestId, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return ServiceResult.Failure(
                    "UserNotAuthenticated",
                    "A valid user is required to cancel an adoption request.");
            }

            if (requestId <= 0)
            {
                return ServiceResult.Failure(
                    "InvalidRequestId",
                    "A valid adoption request must be selected.");
            }

            AdoptionRequest? request = await context.AdoptionRequests
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return ServiceResult.Failure(
                    "RequestNotFound",
                    "The adoption request could not be found.");
            }

            if (request.ApplicantId != currentUserId)
            {
                return ServiceResult.Failure(
                    "Unauthorized",
                    "You are not allowed to cancel this adoption request.");
            }

            if (request.Status != AdoptionRequestStatus.Pending)
            {
                return ServiceResult.Failure(
                    "RequestNotPending",
                    "Only pending adoption requests can be cancelled.");
            }

            request.Status = AdoptionRequestStatus.Cancelled;

            await context.SaveChangesAsync();

            return ServiceResult.Success();
        }


        public async Task<ServiceResult> RejectAsync(int requestId, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return ServiceResult.Failure(
                    "UserNotAuthenticated",
                    "A valid user is required to reject an adoption request.");
            }

            if (requestId <= 0)
            {
                return ServiceResult.Failure(
                    "InvalidRequestId",
                    "A valid adoption request must be selected.");
            }

            AdoptionRequest? request = await context.AdoptionRequests
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return ServiceResult.Failure(
                    "RequestNotFound",
                    "The adoption request could not be found.");
            }

            if (request.Pet.OwnerId != currentUserId)
            {
                return ServiceResult.Failure(
                    "Unauthorized",
                    "You are not allowed to reject this adoption request.");
            }

            if (request.Status != AdoptionRequestStatus.Pending)
            {
                return ServiceResult.Failure(
                    "RequestNotPending",
                    "Only pending adoption requests can be rejected.");
            }

            request.Status = AdoptionRequestStatus.Rejected;

            await context.SaveChangesAsync();

            return ServiceResult.Success();
        }


        public async Task<ServiceResult> ApproveAsync(int requestId, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return ServiceResult.Failure(
                    "UserNotAuthenticated",
                    "A valid user is required to approve an adoption request.");
            }

            if (requestId <= 0)
            {
                return ServiceResult.Failure(
                    "InvalidRequestId",
                    "A valid adoption request must be selected.");
            }

            AdoptionRequest? request = await context.AdoptionRequests
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return ServiceResult.Failure(
                    "RequestNotFound",
                    "The adoption request could not be found.");
            }

            if (request.Pet.OwnerId != currentUserId)
            {
                return ServiceResult.Failure(
                    "Unauthorized",
                    "You are not allowed to approve this adoption request.");
            }

            if (request.Status != AdoptionRequestStatus.Pending)
            {
                return ServiceResult.Failure(
                    "RequestNotPending",
                    "Only pending adoption requests can be approved.");
            }

            if (request.Pet.Status != PetStatus.Available)
            {
                return ServiceResult.Failure(
                    "PetNotAvailable",
                    "This pet is no longer available for adoption.");
            }

            await using IDbContextTransaction transaction =
                await context.Database.BeginTransactionAsync();

            request.Status = AdoptionRequestStatus.Approved;
            request.Pet.Status = PetStatus.Adopted;

            IEnumerable<AdoptionRequest> otherPendingRequests = await context.AdoptionRequests
                .Where(r =>
                    r.PetId == request.PetId &&
                    r.Id != request.Id &&
                    r.Status == AdoptionRequestStatus.Pending)
                .ToListAsync();

            foreach (var otherRequest in otherPendingRequests)
            {
                otherRequest.Status = AdoptionRequestStatus.Rejected;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult.Success();
        }

    }
}

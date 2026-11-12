namespace AdoptAPet.Service.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    using Common;

    //Usings from the Data layer
    using Data;
    using Data.Models;
    using Data.Models.Enums;

    //Usings from the service layer
    using Contracts;
    using DTOs;
    using Results;
    using Validators;

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
                    ServiceErrorKeys.ValidationFailed,
                    ServiceErrorMessages.AdoptionRequestInformationRequired);
            }

            string? validationErrorCheck = AdoptionRequestInputValidator.Validate(dto);

            if (validationErrorCheck != null)
            {
                return ServiceResult<int>.Failure(
                    ServiceErrorKeys.ValidationFailed,
                    validationErrorCheck);
            }

            if (string.IsNullOrWhiteSpace(applicantId))
            {
                return ServiceResult<int>.Failure(
                    ServiceErrorKeys.UserNotAuthenticated,
                    ServiceErrorMessages.UserNotAuthenticated);
            }

            Pet? pet = await context.Pets
                .FirstOrDefaultAsync(p => p.Id == dto.PetId);

            if (pet == null)
            {
                return ServiceResult<int>.Failure(
                    ServiceErrorKeys.PetNotFound,
                    ServiceErrorMessages.PetNotFound);
            }

            if (pet.OwnerId == applicantId)
            {
                return ServiceResult<int>.Failure(
                    ServiceErrorKeys.CannotAdoptOwnPet,
                    ServiceErrorMessages.CannotAdoptOwnPet);
            }

            if (pet.Status != PetStatus.Available)
            {
                return ServiceResult<int>.Failure(
                    ServiceErrorKeys.PetNotAvailable,
                    ServiceErrorMessages.PetNotAvailable);
            }

            bool applicantExists = await context.Users
                .AnyAsync(u => u.Id == applicantId);

            if (!applicantExists)
            {
                return ServiceResult<int>.Failure(
                    ServiceErrorKeys.UserNotFound,
                    ServiceErrorMessages.UserNotFound);
            }

            bool hasPendingRequest = await context.AdoptionRequests
                .AnyAsync(r =>
                    r.PetId == dto.PetId &&
                    r.ApplicantId == applicantId &&
                    r.Status == AdoptionRequestStatus.Pending);

            if (hasPendingRequest)
            {
                return ServiceResult<int>.Failure(
                    ServiceErrorKeys.DuplicateRequest,
                    ServiceErrorMessages.DuplicateRequest);
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
                        ServiceErrorKeys.UserNotAuthenticated,
                        ServiceErrorMessages.UserNotAuthenticated);
            }

            IReadOnlyCollection<AdoptionRequestDto> requests = await context.AdoptionRequests
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
                        ServiceErrorKeys.UserNotAuthenticated,
                        ServiceErrorMessages.UserNotAuthenticated);
            }

            if (petId <= 0)
            {
                return ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>
                    .Failure(
                        ServiceErrorKeys.InvalidPetId,
                        ServiceErrorMessages.InvalidPetId);
            }

            bool petExists = await context.Pets
                .AnyAsync(p => p.Id == petId && p.OwnerId == ownerId);

            if (!petExists)
            {
                return ServiceResult<IReadOnlyCollection<AdoptionRequestDto>>
                    .Failure(
                        ServiceErrorKeys.PetNotFound,
                        ServiceErrorMessages.PetNotFound);
            }

            IReadOnlyCollection<AdoptionRequestDto> requests = await context.AdoptionRequests
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
                    ServiceErrorKeys.UserNotAuthenticated,
                    ServiceErrorMessages.UserNotAuthenticated);
            }

            if (requestId <= 0)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.InvalidRequestId,
                    ServiceErrorMessages.InvalidRequestId);
            }

            AdoptionRequest? request = await context.AdoptionRequests
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.RequestNotFound,
                    ServiceErrorMessages.RequestNotFound);
            }

            if (request.ApplicantId != currentUserId)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.Unauthorized,
                    ServiceErrorMessages.Unauthorized);
            }

            if (request.Status != AdoptionRequestStatus.Pending)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.RequestNotPending,
                    ServiceErrorMessages.RequestCannotBeCancelled);
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
                    ServiceErrorKeys.UserNotAuthenticated,
                    ServiceErrorMessages.UserNotAuthenticated);
            }

            if (requestId <= 0)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.InvalidRequestId,
                    ServiceErrorMessages.InvalidRequestId);
            }

            AdoptionRequest? request = await context.AdoptionRequests
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.RequestNotFound,
                    ServiceErrorMessages.RequestNotFound);
            }

            if (request.Pet.OwnerId != currentUserId)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.Unauthorized,
                    ServiceErrorMessages.Unauthorized);
            }

            if (request.Status != AdoptionRequestStatus.Pending)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.RequestNotPending,
                    ServiceErrorMessages.RequestCannotBeRejected);
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
                    ServiceErrorKeys.UserNotAuthenticated,
                    ServiceErrorMessages.UserNotAuthenticated);
            }

            if (requestId <= 0)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.InvalidRequestId,
                    ServiceErrorMessages.InvalidRequestId);
            }

            AdoptionRequest? request = await context.AdoptionRequests
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.RequestNotFound,
                    ServiceErrorMessages.RequestNotFound);
            }

            if (request.Pet.OwnerId != currentUserId)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.Unauthorized,
                    ServiceErrorMessages.Unauthorized);
            }

            if (request.Status != AdoptionRequestStatus.Pending)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.RequestNotPending,
                    ServiceErrorMessages.RequestCannotBeApproved);
            }

            if (request.Pet.Status != PetStatus.Available)
            {
                return ServiceResult.Failure(
                    ServiceErrorKeys.PetNotAvailable,
                    ServiceErrorMessages.PetNotAvailable);
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
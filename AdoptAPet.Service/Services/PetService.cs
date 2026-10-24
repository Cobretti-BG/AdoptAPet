namespace AdoptAPet.Service.Services
{
    using Microsoft.EntityFrameworkCore;

    //Usings from the data layer
    using Data;
    using Data.Models;
    using Data.Models.Enums;

    //Usings from the service layer
    using Contracts;
    using DTOs;
    using Results;
    using Validators;
    public class PetService : IPetService
    {
        private readonly ApplicationDbContext context;
        public PetService(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<ServiceResult<PetDto>> GetByIdAsync(int id)
        {
            Pet? pet = await context.Pets
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
            {
                return ServiceResult<PetDto>.Failure(
                    "PetNotFound",
                    "The requested pet could not be found.");
            }

            return ServiceResult<PetDto>.Success(MapToDto(pet));
        }

        public async Task<ServiceResult<IReadOnlyCollection<PetDto>>> GetAvailableAsync()
        {
            List<Pet> pets = await context.Pets
                .AsNoTracking()
                .Where(p => p.Status == PetStatus.Available)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();

            List<PetDto> petDtos = pets.Select(MapToDto).ToList();

            return ServiceResult<IReadOnlyCollection<PetDto>>
                .Success(petDtos);
        }

        public async Task<ServiceResult<IReadOnlyCollection<PetDto>>> GetByOwnerAsync(string ownerId)
        {
            List<Pet> pets = await context.Pets
                .AsNoTracking()
                .Where(p => p.OwnerId == ownerId)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();

            List<PetDto> petDtos = pets.Select(MapToDto).ToList();

            return ServiceResult<IReadOnlyCollection<PetDto>>
                .Success(petDtos);
        }

        public async Task<ServiceResult<int>> CreateAsync(CreatePetDto dto, string ownerId)
        {
            if (dto == null)
            {
                return ServiceResult<int>.Failure(
                    "ValidationFailed",
                    "Pet information is required.");
            }

            string? validationErrorCheck = PetInputValidator.Validate(dto);

            if (validationErrorCheck != null)
            {
                return ServiceResult<int>.Failure(
                    "ValidationFailed",
                    validationErrorCheck);
            }

            if (string.IsNullOrWhiteSpace(ownerId))
            {
                return ServiceResult<int>.Failure(
                    "UserNotAuthenticated",
                    "A valid user is required to create a pet listing.");
            }

            bool ownerExists = await context.Users
                .AnyAsync(u => u.Id == ownerId);

            if (!ownerExists)
            {
                return ServiceResult<int>.Failure(
                    "UserNotFound",
                    "The specified user could not be found.");
            }

            Pet pet = new Pet
            {
                Name = NormalizeOptionalText(dto.Name),
                Species = dto.Species.Trim(),
                Breed = NormalizeOptionalText(dto.Breed),
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Color = NormalizeOptionalText(dto.Color),
                Description = dto.Description.Trim(),
                Location = dto.Location.Trim(),
                ImageUrl = NormalizeOptionalText(dto.ImageUrl),
                Status = PetStatus.Available,
                CreatedOn = DateTime.UtcNow,
                OwnerId = ownerId
            };

            context.Pets.Add(pet);

            await context.SaveChangesAsync();

            return ServiceResult<int>.Success(pet.Id);
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdatePetDto dto, string currentUserId)
        {

            string? validationErrorCheck = PetInputValidator.Validate(dto);

            if (validationErrorCheck != null)
            {
                return ServiceResult.Failure(
                    "ValidationFailed",
                    validationErrorCheck);
            }

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return ServiceResult.Failure(
                    "UserNotAuthenticated",
                    "A valid user is required to update a pet listing.");
            }

            Pet? pet = await context.Pets
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
            {
                return ServiceResult.Failure(
                    "PetNotFound",
                    "The requested pet could not be found.");
            }

            if (pet.OwnerId != currentUserId)
            {
                return ServiceResult.Failure(
                    "Unauthorized",
                    "You are not allowed to update this pet listing.");
            }

            if (pet.Status != PetStatus.Available)
            {
                return ServiceResult.Failure(
                    "PetNotAvailable",
                    "Only available pet listings can be updated.");
            }

            pet.Name = NormalizeOptionalText(dto.Name);
            pet.Species = dto.Species.Trim();
            pet.Breed = NormalizeOptionalText(dto.Breed);
            pet.Gender = dto.Gender;
            pet.DateOfBirth = dto.DateOfBirth;
            pet.Color = NormalizeOptionalText(dto.Color);
            pet.Description = dto.Description.Trim();
            pet.Location = dto.Location.Trim();
            pet.ImageUrl = NormalizeOptionalText(dto.ImageUrl);

            await context.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return ServiceResult.Failure(
                    "UserNotAuthenticated",
                    "A valid user is required to delete a pet listing.");
            }

            Pet? pet = await context.Pets
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
            {
                return ServiceResult.Failure(
                    "PetNotFound",
                    "The requested pet could not be found.");
            }

            if (pet.OwnerId != currentUserId)
            {
                return ServiceResult.Failure(
                    "Unauthorized",
                    "You are not allowed to delete this pet listing.");
            }

            if (pet.Status != PetStatus.Available)
            {
                return ServiceResult.Failure(
                    "PetNotAvailable",
                    "Only available pet listings can be deleted.");
            }

            if (await context.AdoptionRequests.AnyAsync(r => r.PetId == id))
            {
                return ServiceResult.Failure(
                    "PetHasAdoptionRequests",
                    "This pet cannot be deleted because adoption requests exist.");
            }

            context.Pets.Remove(pet);

            await context.SaveChangesAsync();

            return ServiceResult.Success();
        }



        private static PetDto MapToDto(Pet pet)
        {
            return new PetDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                Gender = pet.Gender,
                DateOfBirth = pet.DateOfBirth,
                Color = pet.Color,
                Description = pet.Description,
                Location = pet.Location,
                ImageUrl = pet.ImageUrl,
                Status = pet.Status,
                CreatedOn = pet.CreatedOn,
                OwnerId = pet.OwnerId
            };
        }
        private static string? NormalizeOptionalText(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}
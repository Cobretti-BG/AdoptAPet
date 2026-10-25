namespace AdoptAPet.Web.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Data.Models.Enums;
    using Service.Contracts;
    using Service.DTOs;

    using ViewModels.Pets;
    using AdoptAPet.Service.Results;

    public class PetsController : Controller
    {
        private readonly IPetService petService;

        public PetsController(IPetService petService)
        {
            this.petService = petService;
        }

        public async Task<IActionResult> Index()
        {
            ServiceResult<IReadOnlyCollection<PetDto>> result = await petService.GetAvailableAsync();

            if (!result.IsSuccess || result.Data == null)
            {
                return View("Error");
            }

            IEnumerable<PetListingViewModel> pets = result.Data
                .Select(pet => new PetListingViewModel
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    Species = pet.Species,
                    Breed = pet.Breed,
                    Gender = pet.Gender,
                    DateOfBirth = pet.DateOfBirth,
                    Location = pet.Location,
                    ImageUrl = pet.ImageUrl
                })
                .ToList();

            return View(pets);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyPets()
        {
            string currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Challenge();
            }

            ServiceResult<IReadOnlyCollection<PetDto>> result = await petService.GetByOwnerAsync(currentUserId);

            if (!result.IsSuccess || result.Data == null)
            {
                return View("Error");
            }

            IEnumerable<PetListingViewModel> viewModels = result.Data.Select(pet => new PetListingViewModel
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                Gender = pet.Gender,
                DateOfBirth = pet.DateOfBirth,
                Location = pet.Location,
                ImageUrl = pet.ImageUrl
            }).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ServiceResult<PetDto> result = await petService.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            PetDto petDto = result.Data;

            PetDetailsViewModel viewModel = new PetDetailsViewModel
            {
                Id = petDto.Id,
                IsOwner = GetCurrentUserId() == petDto.OwnerId,
                Name = petDto.Name,
                Species = petDto.Species,
                Breed = petDto.Breed,
                Gender = petDto.Gender,
                DateOfBirth = petDto.DateOfBirth,
                Color = petDto.Color,
                Description = petDto.Description,
                Location = petDto.Location,
                ImageUrl = petDto.ImageUrl,
                Status = petDto.Status
            };

            return View(viewModel);
        }


        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new PetFormViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreatePetDto dto = new CreatePetDto
            {
                Name = model.Name,
                Species = model.Species,
                Breed = model.Breed,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Color = model.Color,
                Description = model.Description,
                Location = model.Location,
                ImageUrl = model.ImageUrl
            };

            string? userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            ServiceResult<int> result = await petService.CreateAsync(dto, userId);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage ?? "The pet could not be created.");

                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = result.Data });
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ServiceResult<PetDto> result = await petService.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            PetDto pet = result.Data;

            string? userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            if (pet.OwnerId != userId)
            {
                return Forbid();
            }

            if (pet.Status != PetStatus.Available)
            {
                return BadRequest("Only available pets can be edited.");
            }

            PetFormViewModel model = new PetFormViewModel
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
                ImageUrl = pet.ImageUrl
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PetFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            UpdatePetDto dto = new UpdatePetDto
            {
                Name = model.Name,
                Species = model.Species,
                Breed = model.Breed,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Color = model.Color,
                Description = model.Description,
                Location = model.Location,
                ImageUrl = model.ImageUrl
            };

            ServiceResult result = await petService.UpdateAsync(id, dto, userId);

            if (!result.IsSuccess)
            {
                if (result.ErrorKey == "PetNotFound")
                {
                    return NotFound();
                }

                if (result.ErrorKey == "Unauthorized")
                {
                    return Forbid();
                }

                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage ?? "The pet could not be updated.");

                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ServiceResult<PetDto> result = await petService.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            PetDto pet = result.Data;

            string? userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            if (pet.OwnerId != userId)
            {
                return Forbid();
            }

            if (pet.Status != PetStatus.Available)
            {
                return BadRequest("Only available pets can be deleted.");
            }

            PetDetailsViewModel model = new PetDetailsViewModel
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
                Status = pet.Status
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            ServiceResult result = await petService.DeleteAsync(id, userId);

            if (!result.IsSuccess)
            {
                if (result.ErrorKey == "PetNotFound")
                {
                    return NotFound();
                }

                if (result.ErrorKey == "Unauthorized")
                {
                    return Forbid();
                }

                TempData["ErrorMessage"] =
                    result.ErrorMessage ?? "The pet could not be deleted.";

                return RedirectToAction(nameof(Details), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}

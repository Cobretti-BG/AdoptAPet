namespace AdoptAPet.Web.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Data.Models.Enums;

    using Service.Contracts;
    using Service.DTOs;
    using Service.Results;

    using Common;

    using ViewModels.AdoptionRequests;

    [Authorize]
    public class AdoptionRequestsController : Controller
    {
        private readonly IAdoptionRequestService adoptionRequestService;
        private readonly IPetService petService;

        public AdoptionRequestsController(IAdoptionRequestService adoptionRequestService, IPetService petService)
        {
            this.adoptionRequestService = adoptionRequestService;
            this.petService = petService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int petId)
        {
            if (petId <= 0)
            {
                return NotFound();
            }

            ServiceResult<PetDto> result = await petService.GetByIdAsync(petId);

            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            if (result.Data.Status != PetStatus.Available)
            {
                TempData["ErrorMessage"] =
                    "This pet is no longer available for adoption.";

                return RedirectToAction(
                    "Details",
                    "Pets",
                    new { id = petId });
            }

            string? currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Challenge();
            }

            if (result.Data.OwnerId == currentUserId)
            {
                TempData["ErrorMessage"] = "You can't apply for adoption of your own pets.";
                return RedirectToAction("MyPets", "Pets");
            }

            CreateAdoptionRequestViewModel model = new CreateAdoptionRequestViewModel
            {
                PetId = result.Data.Id
            };

            ViewData["PetName"] = string.IsNullOrWhiteSpace(result.Data.Name)
                ? result.Data.Species
                : result.Data.Name;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdoptionRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ServiceResult<PetDto> petResult = await petService.GetByIdAsync(model.PetId);

                if (petResult.IsSuccess && petResult.Data != null)
                {
                    ViewData["PetName"] = string.IsNullOrWhiteSpace(petResult.Data.Name)
                            ? petResult.Data.Species
                            : petResult.Data.Name;
                }

                return View(model);
            }

            string? currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Challenge();
            }

            CreateAdoptionRequestDto dto = new CreateAdoptionRequestDto
            {
                PetId = model.PetId,
                Message = model.Message
            };

            ServiceResult<int> result = await adoptionRequestService.CreateAsync(
                dto,
                currentUserId);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage ?? "The adoption request could not be submitted.");

                ServiceResult<PetDto> petResult = await petService.GetByIdAsync(model.PetId);

                if (petResult.IsSuccess && petResult.Data != null)
                {
                    ViewData["PetName"] = string.IsNullOrWhiteSpace(petResult.Data.Name)
                            ? petResult.Data.Species
                            : petResult.Data.Name;
                }

                return View(model);
            }

            TempData["SuccessMessage"] = "Your adoption request has been submitted.";

            return RedirectToAction(nameof(MyRequests));
        }

        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            string? currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Challenge();
            }

            ServiceResult<IReadOnlyCollection<AdoptionRequestDto>> result = await adoptionRequestService
                .GetByApplicantAsync(currentUserId);

            if (!result.IsSuccess || result.Data == null)
            {
                return View("Error");
            }

            IEnumerable<AdoptionRequestViewModel> viewModels = result.Data
                .Select(request => new AdoptionRequestViewModel
                {
                    Id = request.Id,
                    PetId = request.PetId,
                    PetSpecies = request.PetSpecies,
                    PetName = request.PetName,
                    ApplicantUserName = request.ApplicantUserName,
                    Message = request.Message,
                    Status = request.Status,
                    CreatedOn = request.CreatedOn
                })
                .ToList();

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> ForMyPets(int petId)
        {
            if (petId <= 0)
            {
                return NotFound();
            }

            string? currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Challenge();
            }

            ServiceResult<IReadOnlyCollection<AdoptionRequestDto>> result = await adoptionRequestService
                .GetByPetOwnerAsync(currentUserId, petId);

            if (!result.IsSuccess || result.Data == null)
            {
                if (result.ErrorKey == ServiceErrorKeys.PetNotFound)
                {
                    return NotFound();
                }

                if (result.ErrorKey == ServiceErrorKeys.Unauthorized)
                {
                    return Forbid();
                }

                return View("Error");
            }

            IEnumerable<AdoptionRequestViewModel> viewModels = result.Data
                .Select(request => new AdoptionRequestViewModel
                {
                    Id = request.Id,
                    PetId = request.PetId,
                    PetSpecies = request.PetSpecies,
                    PetName = request.PetName,
                    ApplicantUserName = request.ApplicantUserName,
                    Message = request.Message,
                    Status = request.Status,
                    CreatedOn = request.CreatedOn
                })
                .ToList();

            ViewData["PetId"] = petId;

            return View(viewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int requestId)
        {
            if (requestId <= 0)
            {
                return NotFound();
            }
            string? currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Challenge();
            }

            ServiceResult result = await adoptionRequestService.CancelAsync(
                requestId,
                currentUserId);

            if (!result.IsSuccess)
            {
                if (result.ErrorKey == ServiceErrorKeys.RequestNotFound)
                {
                    return NotFound();
                }

                if (result.ErrorKey == ServiceErrorKeys.Unauthorized)
                {
                    return Forbid();
                }

                TempData["ErrorMessage"] = result.ErrorMessage
                    ?? "The adoption request could not be cancelled.";

                return RedirectToAction(nameof(MyRequests));
            }

            TempData["SuccessMessage"] = "Your adoption request has been cancelled.";

            return RedirectToAction(nameof(MyRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int requestId, int petId)
        {
            if (requestId <= 0 || petId <= 0)
            {
                return NotFound();
            }
            string? currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Challenge();
            }

            ServiceResult result = await adoptionRequestService.ApproveAsync(
                requestId,
                currentUserId);

            if (!result.IsSuccess)
            {
                if (result.ErrorKey == ServiceErrorKeys.RequestNotFound)
                {
                    return NotFound();
                }

                if (result.ErrorKey == ServiceErrorKeys.Unauthorized)
                {
                    return Forbid();
                }

                TempData["ErrorMessage"] = result.ErrorMessage
                    ?? "The adoption request could not be approved.";

                return RedirectToAction(nameof(ForMyPets), new { petId });
            }

            TempData["SuccessMessage"] =
                "The adoption request was approved! This pet is now marked as having lovely home. :)";

            return RedirectToAction(nameof(ForMyPets), new { petId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int requestId, int petId)
        {
            if (requestId <= 0 || petId <= 0)
            {
                return NotFound();
            }
            string? currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Challenge();
            }

            ServiceResult result = await adoptionRequestService.RejectAsync(
                requestId,
                currentUserId);

            if (!result.IsSuccess)
            {
                if (result.ErrorKey == ServiceErrorKeys.RequestNotFound)
                {
                    return NotFound();
                }

                if (result.ErrorKey == ServiceErrorKeys.Unauthorized)
                {
                    return Forbid();
                }

                TempData["ErrorMessage"] = result.ErrorMessage
                    ?? "The adoption request could not be rejected.";

                return RedirectToAction(nameof(ForMyPets), new { petId });
            }

            TempData["SuccessMessage"] = "The adoption request has been rejected.";

            return RedirectToAction(nameof(ForMyPets), new { petId });
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}

namespace AdoptAPet.Service.Validators
{
    using Data.Models.Enums;
    
    using DTOs;

    using static Common.EntityValidation.Pet;

    internal static class PetInputValidator
    {
        public static string? Validate(CreatePetDto dto)
        {
            return Validate(
                dto.Name,
                dto.Species,
                dto.Breed,
                dto.Gender,
                dto.DateOfBirth,
                dto.Color,
                dto.Description,
                dto.Location,
                dto.ImageUrl);
        }

        public static string? Validate(UpdatePetDto dto)
        {
            return Validate(
                dto.Name,
                dto.Species,
                dto.Breed,
                dto.Gender,
                dto.DateOfBirth,
                dto.Color,
                dto.Description,
                dto.Location,
                dto.ImageUrl);
        }

        private static string? Validate(
            string? name,
            string? species,
            string? breed,
            PetGender gender,
            DateTime? dateOfBirth,
            string? color,
            string? description,
            string? location,
            string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(species))
            {
                return "Species field is required.";
            }

            if (species.Trim().Length < SpeciesMinLength)
            {
                return $"Species must be minimum {SpeciesMinLength} letters long.";
            }

            if (species.Trim().Length > SpeciesMaxLength)
            {
                return $"Species maximum length is {SpeciesMaxLength} letters.";
            }

            if (name != null && name.Trim().Length < NameMinLength)
            {
                return $"Name must be minimum {NameMinLength} letters long.";
            }

            if (name != null && name.Trim().Length > NameMaxLength)
            {
                return $"Name maximum length is {NameMaxLength} letters.";
            }

            if (breed?.Trim().Length > BreedMaxLength)
            {
                return $"Breed is too long. Maximum length is {BreedMaxLength} letters.";
            }

            if (color?.Trim().Length > ColorMaxLength)
            {
                return "Color value is too long. Please enter valid color.";
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return "Description is required.";
            }

            if (description.Trim().Length < DescriptionMinLength)
            {
                return $"The description is too short. Minimum required length is {DescriptionMinLength} letters.";
            }

            if (description.Trim().Length > DescriptionMaxLength)
            {
                return $"Description is too long. Maximum allowed length is {DescriptionMaxLength} characters";
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                return "Location is required.";
            }

            if (location.Trim().Length < LocationMinLength)
            {
                return $"Location minimum length is {LocationMinLength} letters.";
            }

            if (location.Trim().Length > LocationMaxLength)
            {
                return $"Location maximum length is {LocationMaxLength} letters.";
            }

            if (imageUrl?.Trim().Length > ImageUrlMaxLength)
            {
                return "Image URL is too long. Please enter valid image URL.";
            }

            if (imageUrl != null &&
                !string.IsNullOrWhiteSpace(imageUrl) &&
                (!Uri.TryCreate(imageUrl.Trim(), UriKind.Absolute, out Uri uri) ||
                 (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
            {
                return "Image URL must be a valid HTTP or HTTPS URL.";
            }

            if (!Enum.IsDefined(gender))
            {
                return "The selected pet gender is invalid.";
            }

            if (dateOfBirth.HasValue &&
                dateOfBirth.Value.Date > DateTime.UtcNow.Date)
            {
                return "The date of birth cannot be in the future. Right? :)";
            }

            return null;
        }
    }
}

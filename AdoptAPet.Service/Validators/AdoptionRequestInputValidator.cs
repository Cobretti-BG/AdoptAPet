namespace AdoptAPet.Service.Validators
{
    using static Common.EntityValidation.AdoptionRequest;

    using DTOs;

    internal static class AdoptionRequestInputValidator
    {
        public static string? Validate(CreateAdoptionRequestDto dto)
        {
            if (dto.PetId <= 0)
            {
                return "A valid pet must be selected.";
            }

            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                return "An adoption request message is required.";
            }

            int messageLength = dto.Message.Trim().Length;

            if (messageLength < MessageMinLength)
            {
                return $"The adoption request message must be at least {MessageMinLength} characters long.";
            }

            if (messageLength > MessageMaxLength)
            {
                return $"The adoption request message cannot exceed {MessageMaxLength} characters.";
            }

            return null;
        }
    }
}

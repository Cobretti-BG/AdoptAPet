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
                return "A message is required.";
            }

            int messageLength = dto.Message.Trim().Length;

            if (messageLength < MessageMinLength)
            {
                return $"The adoption message has to be minimum {MessageMinLength} characters.";
            }

            if (messageLength > MessageMaxLength)
            {
                return $"The adoption message cannot exceed {MessageMaxLength} characters.";
            }

            return null;
        }
    }
}

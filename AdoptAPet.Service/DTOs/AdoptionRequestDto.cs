namespace AdoptAPet.Service.DTOs
{
    using Data.Models.Enums;
    public class AdoptionRequestDto
    {
        public int Id { get; set; }

        public int PetId { get; set; }

        public string PetSpecies { get; set; } = string.Empty;

        public string? PetName { get; set; }

        public string ApplicantId { get; set; } = string.Empty;

        public string? ApplicantUserName { get; set; }

        public string Message { get; set; } = string.Empty;

        public AdoptionRequestStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

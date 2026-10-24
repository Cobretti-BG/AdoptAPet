namespace AdoptAPet.Data.Models
{
    using Enums;
    public class AdoptionRequest
    {
        public int Id { get; set; }

        public int PetId { get; set; }

        public Pet Pet { get; set; } = null!;

        public string ApplicantId { get; set; } = string.Empty;

        public ApplicationUser Applicant { get; set; } = null!;

        public string Message { get; set; } = string.Empty;

        public AdoptionRequestStatus Status { get; set; }
            = AdoptionRequestStatus.Pending;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}

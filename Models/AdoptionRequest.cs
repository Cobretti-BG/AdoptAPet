namespace AdoptAPet.Models
{
    using Enums;
    public class AdoptionRequest
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public Pet Pet { get; set; } = null!;
        public int ApplicantId { get; set; }
        public User Applicant { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public AdoptionRequestStatus Status { get; set; }
    }
}

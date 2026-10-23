using AdoptAPet.Data.Models.Enums;

namespace AdoptAPet.Data.Models
{
    public class Pet
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string Species { get; set; } = string.Empty;

        public string? Breed { get; set; }

        public PetGender Gender { get; set; } = PetGender.Unknown;

        public DateTime? DateOfBirth { get; set; }

        public string? Color { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public PetStatus Status { get; set; } = PetStatus.Available;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string OwnerId { get; set; } = string.Empty;

        public ApplicationUser Owner { get; set; } = null!;

        public ICollection<AdoptionRequest> AdoptionRequests { get; set; }
            = new List<AdoptionRequest>();
    }
}

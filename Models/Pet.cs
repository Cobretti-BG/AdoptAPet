namespace AdoptAPet.Models
{
    using Enums;
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string? Breed { get; set; }
        public PetGender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Color { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsAdopted { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; }
            = new List<AdoptionRequest>();
    }
}

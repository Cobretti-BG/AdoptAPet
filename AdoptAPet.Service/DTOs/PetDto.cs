namespace AdoptAPet.Service.DTOs
{
    using Data.Models.Enums;
    public class PetDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string Species { get; set; } = string.Empty;

        public string? Breed { get; set; }

        public PetGender Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Color { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public PetStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public string OwnerId { get; set; } = string.Empty;
    }
}

namespace AdoptAPet.Web.ViewModels.Pets
{
    using Data.Models.Enums;
    public class PetListingViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string Species { get; set; } = string.Empty;

        public string? Breed { get; set; }

        public PetGender Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }
}

namespace AdoptAPet.Web.ViewModels.Pets
{
    using System.ComponentModel.DataAnnotations;

    using Data.Models.Enums;

    using static AdoptAPet.Common.EntityValidation.Pet;
    public class PetFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Pet name")]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string? Name { get; set; }

        [Required]
        [StringLength(SpeciesMaxLength, MinimumLength = SpeciesMinLength)]
        public string Species { get; set; } = string.Empty;

        [StringLength(BreedMaxLength)]
        public string? Breed { get; set; }

        [EnumDataType(typeof(PetGender))]
        [Display(Name = "Gender")]
        public PetGender Gender { get; set; } = PetGender.Unknown;

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(ColorMaxLength)]
        public string? Color { get; set; }

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(LocationMaxLength, MinimumLength = LocationMinLength)]
        public string Location { get; set; } = string.Empty;

        [Url]
        [StringLength(ImageUrlMaxLength)]
        [RegularExpression(
            @"^https?://.+",
            ErrorMessage = "Image URL must start with http:// or https://.")]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
    }
}

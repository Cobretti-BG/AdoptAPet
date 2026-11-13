namespace AdoptAPet.Web.ViewModels.Pets
{
    using System.ComponentModel.DataAnnotations;

    using Data.Models.Enums;
    using static Common.EntityValidation.Pet;

    using Validation;
    public class PetFormViewModel
    {
        public int Id { get; set; }

        private string? name;
        [Display(Name = "Pet name")]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string? Name
        {
            get => name;
            set => name = string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }


        private string species = string.Empty;
        [Required]
        [StringLength(SpeciesMaxLength, MinimumLength = SpeciesMinLength)]
        public string Species
        {
            get => species;
            set => species = value?.Trim() ?? string.Empty;
        }


        private string? breed;
        [StringLength(BreedMaxLength)]
        public string? Breed
        {
            get => breed;
            set => breed = string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }


        [EnumDataType(typeof(PetGender))]
        [Display(Name = "Gender")]
        public PetGender Gender { get; set; } = PetGender.Unknown;


        [DataType(DataType.Date)]
        [NotFutureDate]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }


        private string? color;
        [StringLength(ColorMaxLength)]
        public string? Color
        {
            get => color;
            set => color = string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }


        private string description = string.Empty;
        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
        public string Description
        {
            get => description;
            set => description = value?.Trim() ?? string.Empty;
        }


        private string location = string.Empty;
        [Required]
        [StringLength(LocationMaxLength, MinimumLength = LocationMinLength)]
        public string Location
        {
            get => location;
            set => location = value?.Trim() ?? string.Empty;
        }


        private string? imageUrl;
        [HttpUrl]
        [StringLength(ImageUrlMaxLength)]
        [Display(Name = "Image URL")]
        public string? ImageUrl
        {
            get => imageUrl;
            set => imageUrl = string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}

namespace AdoptAPet.Web.ViewModels.AdoptionRequests
{
    using System.ComponentModel.DataAnnotations;
    
    using static Common.EntityValidation.AdoptionRequest;
    public class CreateAdoptionRequestViewModel
    {
        [Required]
        public int PetId { get; set; }

        [Required]
        [StringLength(MessageMaxLength, MinimumLength = MessageMinLength, 
            ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        [Display(Name = "adoption request message")]
        public string Message { get; set; } = string.Empty;
    }
}

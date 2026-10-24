namespace AdoptAPet.Web.ViewModels.AdoptionRequests
{
    using System.ComponentModel.DataAnnotations;
    
    using static Common.EntityValidation.AdoptionRequest;
    public class CreateAdoptionRequestViewModel
    {
        [Required]
        public int PetId { get; set; }

        [Required]
        [StringLength(MessageMaxLength, MinimumLength = MessageMinLength)]
        [Display(Name = "Say hi, and few things about yourself and your interest in this friend? :)")]
        public string Message { get; set; } = string.Empty;
    }
}

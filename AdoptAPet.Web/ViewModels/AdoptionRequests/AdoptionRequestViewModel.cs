namespace AdoptAPet.Web.ViewModels.AdoptionRequests
{
    using System.ComponentModel.DataAnnotations;
    using Data.Models.Enums;
    public class AdoptionRequestViewModel
    {
        public int Id { get; set; }

        public int PetId { get; set; }

        [Display(Name = "Pet")]
        public string PetSpecies { get; set; } = string.Empty;

        public string? PetName { get; set; }

        public string? ApplicantUserName { get; set; }

        public string Message { get; set; } = string.Empty;

        public AdoptionRequestStatus Status { get; set; }

        [Display(Name = "Submitted on")]
        public DateTime CreatedOn { get; set; }
    }
}

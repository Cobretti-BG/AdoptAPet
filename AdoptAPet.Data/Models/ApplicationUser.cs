namespace AdoptAPet.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Pet> Pets { get; set; }
            = new List<Pet>();
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; }
            = new List<AdoptionRequest>();
    }
}

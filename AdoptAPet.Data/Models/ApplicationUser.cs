using Microsoft.AspNetCore.Identity;

namespace AdoptAPet.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Pet> Pets { get; set; } 
            = new List<Pet>();
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; }
            = new List<AdoptionRequest>();
    }
}

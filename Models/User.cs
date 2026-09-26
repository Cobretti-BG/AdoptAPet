namespace AdoptAPet.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; } 
            = new List<AdoptionRequest>();
    }
}

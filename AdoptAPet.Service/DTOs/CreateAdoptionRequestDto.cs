namespace AdoptAPet.Service.DTOs
{
    public class CreateAdoptionRequestDto
    {
        public int PetId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}

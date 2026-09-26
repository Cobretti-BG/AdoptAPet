namespace AdoptAPet.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class AdoptionRequestConfiguration : IEntityTypeConfiguration<AdoptionRequest>
    {
        public void Configure(EntityTypeBuilder<AdoptionRequest> builder)
        {
            builder
               .HasOne(ar => ar.Pet)
               .WithMany(p => p.AdoptionRequests)
               .HasForeignKey(ar => ar.PetId)
               .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(ar => ar.Applicant)
                .WithMany(u => u.AdoptionRequests)
                .HasForeignKey(ar => ar.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

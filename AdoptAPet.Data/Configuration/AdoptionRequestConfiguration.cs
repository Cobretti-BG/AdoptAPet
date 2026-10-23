using AdoptAPet.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static AdoptAPet.Common.EntityValidation.AdoptionRequest;

namespace AdoptAPet.Data.Configuration
{
    public class AdoptionRequestConfiguration
        : IEntityTypeConfiguration<AdoptionRequest>
    {
        public void Configure(EntityTypeBuilder<AdoptionRequest> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Message)
                .IsRequired()
                .HasMaxLength(MessageMaxLength);

            builder.Property(r => r.Status)
                .IsRequired();

            builder.Property(r => r.CreatedOn)
                .IsRequired();

            builder.HasOne(r => r.Applicant)
                .WithMany(u => u.AdoptionRequests)
                .HasForeignKey(r => r.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

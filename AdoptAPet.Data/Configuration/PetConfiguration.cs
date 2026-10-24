namespace AdoptAPet.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Data.Models;

    using static Common.EntityValidation.Pet;
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(NameMaxLength);

            builder.Property(p => p.Species)
                .IsRequired()
                .HasMaxLength(SpeciesMaxLength);

            builder.Property(p => p.Breed)
                .HasMaxLength(BreedMaxLength);

            builder.Property(p => p.Color)
                .HasMaxLength(ColorMaxLength);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            builder.Property(p => p.Location)
                .IsRequired()
                .HasMaxLength(LocationMaxLength);

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(ImageUrlMaxLength);

            builder.Property(p => p.Gender)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.CreatedOn)
                .IsRequired();

            builder.HasOne(p => p.Owner)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.AdoptionRequests)
                .WithOne(r => r.Pet)
                .HasForeignKey(r => r.PetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

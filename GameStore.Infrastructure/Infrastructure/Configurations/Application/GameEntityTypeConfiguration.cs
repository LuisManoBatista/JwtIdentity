using GameStore.Infrastructure.Models.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Configurations.Application;

public class GameEntityTypeConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games", "dbo", t =>
        {
            t.HasCheckConstraint("CK_Games_Price", "[Price] <= 100");
        });

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("varchar(50)");

        builder.Property(t => t.Genre)
          .IsRequired()
          .HasMaxLength(20)
          .HasColumnType("varchar(20)");


        builder.Property(t => t.Price)
               .HasPrecision(5, 2);

        builder.Property(t => t.ReleaseDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(t => t.ImageUri)
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");

        builder.HasIndex(t => t.Name)
           .IsUnique()
           .HasDatabaseName("IX_Games_Name_Unique");
    }
}
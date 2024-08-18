using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NUlid;
using UrlShortener.Application.ReadModels;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Persistence.Contants;

namespace UrlShortener.Persistence.Configurations.Read
{
    internal sealed class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenUrlReadModel>
    {
        public void Configure(EntityTypeBuilder<ShortenUrlReadModel> builder)
        {
            builder.ToTable(TableNames.ShortenedUrls);

            //
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasMaxLength(26) // ULID max 26 char
                .HasConversion(
                ShortendUrlId => ShortendUrlId.Value.ToString(),
                value => new ShortenedUrlId(Ulid.Parse(value)));
        }
    }
}
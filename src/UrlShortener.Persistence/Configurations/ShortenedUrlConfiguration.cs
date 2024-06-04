using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Persistence.Contants;

namespace UrlShortener.Persistence.Configurations
{
    internal sealed class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
    {
        public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
        {
            builder.ToTable(TableNames.ShortenedUrls);

            //TODO: 需要與計算後欄位一致
            //builder.Property(s => s.Code).HasMaxLength(7);

            //
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasConversion(
                ShortendUrlId => ShortendUrlId.Value,
                value => new ShortenedUrlId(value));

            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
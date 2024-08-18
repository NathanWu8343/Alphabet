using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Serialization;
using UrlShortener.Persistence.Contants;
using UrlShortener.Persistence.Models;

namespace UrlShortener.Persistence.Configurations.Write
{
    public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
    {
        public void Configure(EntityTypeBuilder<AuditTrail> builder)
        {
            // 創建字典到 JSON 字符串的轉換器
            var dictionaryToJsonConverter = new ValueConverter<Dictionary<string, object?>, string>(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!
            );

            // 創建列表到 JSON 字符串的轉換器
            var listToJsonConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!
            );

            builder.ToTable(TableNames.AuditTrails);
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.EntityName);

            builder.Property(e => e.Id);

            builder.Property(e => e.UserId).IsRequired(false);
            builder.Property(e => e.EntityName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.DateAtUtc).IsRequired();
            builder.Property(e => e.PrimaryKey).HasMaxLength(100);

            builder.Property(e => e.TrailType)
                .HasConversion<string>()
                .HasMaxLength(50) // 設置長度限制以避免超出存儲空間
                .IsRequired();

            builder.Property(e => e.ChangedColumns)
                .HasConversion(listToJsonConverter) // 將 List<string> 轉換為 JSON 存儲
                .HasColumnType("text") // 設置合適的列類型
                .IsRequired();

            builder.Property(e => e.OldValues).HasConversion(dictionaryToJsonConverter);
            builder.Property(e => e.NewValues).HasConversion(dictionaryToJsonConverter);

            // 如果需要配置關聯屬性，可以取消註解
            // builder.HasOne(e => e.User)
            //     .WithMany()
            //     .HasForeignKey(e => e.UserId)
            //     .IsRequired(false)
            //     .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
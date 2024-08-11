﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace UrlShortener.Persistence
{
    public partial class ApplicationWriteDbContextModel
    {
        partial void Initialize()
        {
            var shortenedUrl = ShortenedUrlEntityType.Create(this);
            var outboxMessage = OutboxMessageEntityType.Create(this);
            var outboxMessageConsumer = OutboxMessageConsumerEntityType.Create(this);

            ShortenedUrlEntityType.CreateAnnotations(shortenedUrl);
            OutboxMessageEntityType.CreateAnnotations(outboxMessage);
            OutboxMessageConsumerEntityType.CreateAnnotations(outboxMessageConsumer);

            AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
            AddAnnotation("ProductVersion", "8.0.2");
            AddAnnotation("Relational:MaxIdentifierLength", 64);
            AddRuntimeAnnotation("Relational:RelationalModel", CreateRelationalModel());
        }

        private IRelationalModel CreateRelationalModel()
        {
            var relationalModel = new RelationalModel(this);

            var shortenedUrl = FindEntityType("UrlShortener.Domain.Entities.ShortenedUrl")!;

            var defaultTableMappings = new List<TableMappingBase<ColumnMappingBase>>();
            shortenedUrl.SetRuntimeAnnotation("Relational:DefaultMappings", defaultTableMappings);
            var urlShortenerDomainEntitiesShortenedUrlTableBase = new TableBase("UrlShortener.Domain.Entities.ShortenedUrl", null, relationalModel);
            var codeColumnBase = new ColumnBase<ColumnMappingBase>("Code", "varchar(255)", urlShortenerDomainEntitiesShortenedUrlTableBase);
            urlShortenerDomainEntitiesShortenedUrlTableBase.Columns.Add("Code", codeColumnBase);
            var createdAtUtcColumnBase = new ColumnBase<ColumnMappingBase>("CreatedAtUtc", "datetime(6)", urlShortenerDomainEntitiesShortenedUrlTableBase);
            urlShortenerDomainEntitiesShortenedUrlTableBase.Columns.Add("CreatedAtUtc", createdAtUtcColumnBase);
            var idColumnBase = new ColumnBase<ColumnMappingBase>("Id", "varchar(26)", urlShortenerDomainEntitiesShortenedUrlTableBase);
            urlShortenerDomainEntitiesShortenedUrlTableBase.Columns.Add("Id", idColumnBase);
            var longUrlColumnBase = new ColumnBase<ColumnMappingBase>("LongUrl", "longtext", urlShortenerDomainEntitiesShortenedUrlTableBase);
            urlShortenerDomainEntitiesShortenedUrlTableBase.Columns.Add("LongUrl", longUrlColumnBase);
            var shortUrlColumnBase = new ColumnBase<ColumnMappingBase>("ShortUrl", "longtext", urlShortenerDomainEntitiesShortenedUrlTableBase);
            urlShortenerDomainEntitiesShortenedUrlTableBase.Columns.Add("ShortUrl", shortUrlColumnBase);
            relationalModel.DefaultTables.Add("UrlShortener.Domain.Entities.ShortenedUrl", urlShortenerDomainEntitiesShortenedUrlTableBase);
            var urlShortenerDomainEntitiesShortenedUrlMappingBase = new TableMappingBase<ColumnMappingBase>(shortenedUrl, urlShortenerDomainEntitiesShortenedUrlTableBase, true);
            urlShortenerDomainEntitiesShortenedUrlTableBase.AddTypeMapping(urlShortenerDomainEntitiesShortenedUrlMappingBase, false);
            defaultTableMappings.Add(urlShortenerDomainEntitiesShortenedUrlMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)idColumnBase, shortenedUrl.FindProperty("Id")!, urlShortenerDomainEntitiesShortenedUrlMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)codeColumnBase, shortenedUrl.FindProperty("Code")!, urlShortenerDomainEntitiesShortenedUrlMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)createdAtUtcColumnBase, shortenedUrl.FindProperty("CreatedAtUtc")!, urlShortenerDomainEntitiesShortenedUrlMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)longUrlColumnBase, shortenedUrl.FindProperty("LongUrl")!, urlShortenerDomainEntitiesShortenedUrlMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)shortUrlColumnBase, shortenedUrl.FindProperty("ShortUrl")!, urlShortenerDomainEntitiesShortenedUrlMappingBase);

            var tableMappings = new List<TableMapping>();
            shortenedUrl.SetRuntimeAnnotation("Relational:TableMappings", tableMappings);
            var shortenedUrlsTable = new Table("ShortenedUrls", null, relationalModel);
            var idColumn = new Column("Id", "varchar(26)", shortenedUrlsTable);
            shortenedUrlsTable.Columns.Add("Id", idColumn);
            var codeColumn = new Column("Code", "varchar(255)", shortenedUrlsTable);
            shortenedUrlsTable.Columns.Add("Code", codeColumn);
            var createdAtUtcColumn = new Column("CreatedAtUtc", "datetime(6)", shortenedUrlsTable);
            shortenedUrlsTable.Columns.Add("CreatedAtUtc", createdAtUtcColumn);
            var longUrlColumn = new Column("LongUrl", "longtext", shortenedUrlsTable);
            shortenedUrlsTable.Columns.Add("LongUrl", longUrlColumn);
            var shortUrlColumn = new Column("ShortUrl", "longtext", shortenedUrlsTable);
            shortenedUrlsTable.Columns.Add("ShortUrl", shortUrlColumn);
            var pK_ShortenedUrls = new UniqueConstraint("PK_ShortenedUrls", shortenedUrlsTable, new[] { idColumn });
            shortenedUrlsTable.PrimaryKey = pK_ShortenedUrls;
            var pK_ShortenedUrlsUc = RelationalModel.GetKey(this,
                "UrlShortener.Domain.Entities.ShortenedUrl",
                new[] { "Id" });
            pK_ShortenedUrls.MappedKeys.Add(pK_ShortenedUrlsUc);
            RelationalModel.GetOrCreateUniqueConstraints(pK_ShortenedUrlsUc).Add(pK_ShortenedUrls);
            shortenedUrlsTable.UniqueConstraints.Add("PK_ShortenedUrls", pK_ShortenedUrls);
            var iX_ShortenedUrls_Code = new TableIndex(
            "IX_ShortenedUrls_Code", shortenedUrlsTable, new[] { codeColumn }, true);
            var iX_ShortenedUrls_CodeIx = RelationalModel.GetIndex(this,
                "UrlShortener.Domain.Entities.ShortenedUrl",
                new[] { "Code" });
            iX_ShortenedUrls_Code.MappedIndexes.Add(iX_ShortenedUrls_CodeIx);
            RelationalModel.GetOrCreateTableIndexes(iX_ShortenedUrls_CodeIx).Add(iX_ShortenedUrls_Code);
            shortenedUrlsTable.Indexes.Add("IX_ShortenedUrls_Code", iX_ShortenedUrls_Code);
            relationalModel.Tables.Add(("ShortenedUrls", null), shortenedUrlsTable);
            var shortenedUrlsTableMapping = new TableMapping(shortenedUrl, shortenedUrlsTable, true);
            shortenedUrlsTable.AddTypeMapping(shortenedUrlsTableMapping, false);
            tableMappings.Add(shortenedUrlsTableMapping);
            RelationalModel.CreateColumnMapping(idColumn, shortenedUrl.FindProperty("Id")!, shortenedUrlsTableMapping);
            RelationalModel.CreateColumnMapping(codeColumn, shortenedUrl.FindProperty("Code")!, shortenedUrlsTableMapping);
            RelationalModel.CreateColumnMapping(createdAtUtcColumn, shortenedUrl.FindProperty("CreatedAtUtc")!, shortenedUrlsTableMapping);
            RelationalModel.CreateColumnMapping(longUrlColumn, shortenedUrl.FindProperty("LongUrl")!, shortenedUrlsTableMapping);
            RelationalModel.CreateColumnMapping(shortUrlColumn, shortenedUrl.FindProperty("ShortUrl")!, shortenedUrlsTableMapping);

            var outboxMessage = FindEntityType("UrlShortener.Persistence.Outbox.OutboxMessage")!;

            var defaultTableMappings0 = new List<TableMappingBase<ColumnMappingBase>>();
            outboxMessage.SetRuntimeAnnotation("Relational:DefaultMappings", defaultTableMappings0);
            var urlShortenerPersistenceOutboxOutboxMessageTableBase = new TableBase("UrlShortener.Persistence.Outbox.OutboxMessage", null, relationalModel);
            var contentColumnBase = new ColumnBase<ColumnMappingBase>("Content", "longtext", urlShortenerPersistenceOutboxOutboxMessageTableBase);
            urlShortenerPersistenceOutboxOutboxMessageTableBase.Columns.Add("Content", contentColumnBase);
            var errorColumnBase = new ColumnBase<ColumnMappingBase>("Error", "longtext", urlShortenerPersistenceOutboxOutboxMessageTableBase)
            {
                IsNullable = true
            };
            urlShortenerPersistenceOutboxOutboxMessageTableBase.Columns.Add("Error", errorColumnBase);
            var idColumnBase0 = new ColumnBase<ColumnMappingBase>("Id", "char(36)", urlShortenerPersistenceOutboxOutboxMessageTableBase);
            urlShortenerPersistenceOutboxOutboxMessageTableBase.Columns.Add("Id", idColumnBase0);
            var occurredAtUtcColumnBase = new ColumnBase<ColumnMappingBase>("OccurredAtUtc", "datetime(6)", urlShortenerPersistenceOutboxOutboxMessageTableBase);
            urlShortenerPersistenceOutboxOutboxMessageTableBase.Columns.Add("OccurredAtUtc", occurredAtUtcColumnBase);
            var processedAtUtcColumnBase = new ColumnBase<ColumnMappingBase>("ProcessedAtUtc", "datetime(6)", urlShortenerPersistenceOutboxOutboxMessageTableBase)
            {
                IsNullable = true
            };
            urlShortenerPersistenceOutboxOutboxMessageTableBase.Columns.Add("ProcessedAtUtc", processedAtUtcColumnBase);
            var typeColumnBase = new ColumnBase<ColumnMappingBase>("Type", "longtext", urlShortenerPersistenceOutboxOutboxMessageTableBase);
            urlShortenerPersistenceOutboxOutboxMessageTableBase.Columns.Add("Type", typeColumnBase);
            relationalModel.DefaultTables.Add("UrlShortener.Persistence.Outbox.OutboxMessage", urlShortenerPersistenceOutboxOutboxMessageTableBase);
            var urlShortenerPersistenceOutboxOutboxMessageMappingBase = new TableMappingBase<ColumnMappingBase>(outboxMessage, urlShortenerPersistenceOutboxOutboxMessageTableBase, true);
            urlShortenerPersistenceOutboxOutboxMessageTableBase.AddTypeMapping(urlShortenerPersistenceOutboxOutboxMessageMappingBase, false);
            defaultTableMappings0.Add(urlShortenerPersistenceOutboxOutboxMessageMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)idColumnBase0, outboxMessage.FindProperty("Id")!, urlShortenerPersistenceOutboxOutboxMessageMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)contentColumnBase, outboxMessage.FindProperty("Content")!, urlShortenerPersistenceOutboxOutboxMessageMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)errorColumnBase, outboxMessage.FindProperty("Error")!, urlShortenerPersistenceOutboxOutboxMessageMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)occurredAtUtcColumnBase, outboxMessage.FindProperty("OccurredAtUtc")!, urlShortenerPersistenceOutboxOutboxMessageMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)processedAtUtcColumnBase, outboxMessage.FindProperty("ProcessedAtUtc")!, urlShortenerPersistenceOutboxOutboxMessageMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)typeColumnBase, outboxMessage.FindProperty("Type")!, urlShortenerPersistenceOutboxOutboxMessageMappingBase);

            var tableMappings0 = new List<TableMapping>();
            outboxMessage.SetRuntimeAnnotation("Relational:TableMappings", tableMappings0);
            var outboxMessagesTable = new Table("OutboxMessages", null, relationalModel);
            var idColumn0 = new Column("Id", "char(36)", outboxMessagesTable);
            outboxMessagesTable.Columns.Add("Id", idColumn0);
            var contentColumn = new Column("Content", "longtext", outboxMessagesTable);
            outboxMessagesTable.Columns.Add("Content", contentColumn);
            var errorColumn = new Column("Error", "longtext", outboxMessagesTable)
            {
                IsNullable = true
            };
            outboxMessagesTable.Columns.Add("Error", errorColumn);
            var occurredAtUtcColumn = new Column("OccurredAtUtc", "datetime(6)", outboxMessagesTable);
            outboxMessagesTable.Columns.Add("OccurredAtUtc", occurredAtUtcColumn);
            var processedAtUtcColumn = new Column("ProcessedAtUtc", "datetime(6)", outboxMessagesTable)
            {
                IsNullable = true
            };
            outboxMessagesTable.Columns.Add("ProcessedAtUtc", processedAtUtcColumn);
            var typeColumn = new Column("Type", "longtext", outboxMessagesTable);
            outboxMessagesTable.Columns.Add("Type", typeColumn);
            var pK_OutboxMessages = new UniqueConstraint("PK_OutboxMessages", outboxMessagesTable, new[] { idColumn0 });
            outboxMessagesTable.PrimaryKey = pK_OutboxMessages;
            var pK_OutboxMessagesUc = RelationalModel.GetKey(this,
                "UrlShortener.Persistence.Outbox.OutboxMessage",
                new[] { "Id" });
            pK_OutboxMessages.MappedKeys.Add(pK_OutboxMessagesUc);
            RelationalModel.GetOrCreateUniqueConstraints(pK_OutboxMessagesUc).Add(pK_OutboxMessages);
            outboxMessagesTable.UniqueConstraints.Add("PK_OutboxMessages", pK_OutboxMessages);
            relationalModel.Tables.Add(("OutboxMessages", null), outboxMessagesTable);
            var outboxMessagesTableMapping = new TableMapping(outboxMessage, outboxMessagesTable, true);
            outboxMessagesTable.AddTypeMapping(outboxMessagesTableMapping, false);
            tableMappings0.Add(outboxMessagesTableMapping);
            RelationalModel.CreateColumnMapping(idColumn0, outboxMessage.FindProperty("Id")!, outboxMessagesTableMapping);
            RelationalModel.CreateColumnMapping(contentColumn, outboxMessage.FindProperty("Content")!, outboxMessagesTableMapping);
            RelationalModel.CreateColumnMapping(errorColumn, outboxMessage.FindProperty("Error")!, outboxMessagesTableMapping);
            RelationalModel.CreateColumnMapping(occurredAtUtcColumn, outboxMessage.FindProperty("OccurredAtUtc")!, outboxMessagesTableMapping);
            RelationalModel.CreateColumnMapping(processedAtUtcColumn, outboxMessage.FindProperty("ProcessedAtUtc")!, outboxMessagesTableMapping);
            RelationalModel.CreateColumnMapping(typeColumn, outboxMessage.FindProperty("Type")!, outboxMessagesTableMapping);

            var outboxMessageConsumer = FindEntityType("UrlShortener.Persistence.Outbox.OutboxMessageConsumer")!;

            var defaultTableMappings1 = new List<TableMappingBase<ColumnMappingBase>>();
            outboxMessageConsumer.SetRuntimeAnnotation("Relational:DefaultMappings", defaultTableMappings1);
            var urlShortenerPersistenceOutboxOutboxMessageConsumerTableBase = new TableBase("UrlShortener.Persistence.Outbox.OutboxMessageConsumer", null, relationalModel);
            var idColumnBase1 = new ColumnBase<ColumnMappingBase>("Id", "char(36)", urlShortenerPersistenceOutboxOutboxMessageConsumerTableBase);
            urlShortenerPersistenceOutboxOutboxMessageConsumerTableBase.Columns.Add("Id", idColumnBase1);
            var nameColumnBase = new ColumnBase<ColumnMappingBase>("Name", "varchar(255)", urlShortenerPersistenceOutboxOutboxMessageConsumerTableBase);
            urlShortenerPersistenceOutboxOutboxMessageConsumerTableBase.Columns.Add("Name", nameColumnBase);
            relationalModel.DefaultTables.Add("UrlShortener.Persistence.Outbox.OutboxMessageConsumer", urlShortenerPersistenceOutboxOutboxMessageConsumerTableBase);
            var urlShortenerPersistenceOutboxOutboxMessageConsumerMappingBase = new TableMappingBase<ColumnMappingBase>(outboxMessageConsumer, urlShortenerPersistenceOutboxOutboxMessageConsumerTableBase, true);
            urlShortenerPersistenceOutboxOutboxMessageConsumerTableBase.AddTypeMapping(urlShortenerPersistenceOutboxOutboxMessageConsumerMappingBase, false);
            defaultTableMappings1.Add(urlShortenerPersistenceOutboxOutboxMessageConsumerMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)idColumnBase1, outboxMessageConsumer.FindProperty("Id")!, urlShortenerPersistenceOutboxOutboxMessageConsumerMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)nameColumnBase, outboxMessageConsumer.FindProperty("Name")!, urlShortenerPersistenceOutboxOutboxMessageConsumerMappingBase);

            var tableMappings1 = new List<TableMapping>();
            outboxMessageConsumer.SetRuntimeAnnotation("Relational:TableMappings", tableMappings1);
            var outboxMessageConsumersTable = new Table("OutboxMessageConsumers", null, relationalModel);
            var idColumn1 = new Column("Id", "char(36)", outboxMessageConsumersTable);
            outboxMessageConsumersTable.Columns.Add("Id", idColumn1);
            var nameColumn = new Column("Name", "varchar(255)", outboxMessageConsumersTable);
            outboxMessageConsumersTable.Columns.Add("Name", nameColumn);
            var pK_OutboxMessageConsumers = new UniqueConstraint("PK_OutboxMessageConsumers", outboxMessageConsumersTable, new[] { idColumn1, nameColumn });
            outboxMessageConsumersTable.PrimaryKey = pK_OutboxMessageConsumers;
            var pK_OutboxMessageConsumersUc = RelationalModel.GetKey(this,
                "UrlShortener.Persistence.Outbox.OutboxMessageConsumer",
                new[] { "Id", "Name" });
            pK_OutboxMessageConsumers.MappedKeys.Add(pK_OutboxMessageConsumersUc);
            RelationalModel.GetOrCreateUniqueConstraints(pK_OutboxMessageConsumersUc).Add(pK_OutboxMessageConsumers);
            outboxMessageConsumersTable.UniqueConstraints.Add("PK_OutboxMessageConsumers", pK_OutboxMessageConsumers);
            relationalModel.Tables.Add(("OutboxMessageConsumers", null), outboxMessageConsumersTable);
            var outboxMessageConsumersTableMapping = new TableMapping(outboxMessageConsumer, outboxMessageConsumersTable, true);
            outboxMessageConsumersTable.AddTypeMapping(outboxMessageConsumersTableMapping, false);
            tableMappings1.Add(outboxMessageConsumersTableMapping);
            RelationalModel.CreateColumnMapping(idColumn1, outboxMessageConsumer.FindProperty("Id")!, outboxMessageConsumersTableMapping);
            RelationalModel.CreateColumnMapping(nameColumn, outboxMessageConsumer.FindProperty("Name")!, outboxMessageConsumersTableMapping);
            return relationalModel.MakeReadOnly();
        }
    }
}

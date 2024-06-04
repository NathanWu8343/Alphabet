using SharedKernel.Common;
using System.Diagnostics.CodeAnalysis;

namespace SharedKernel.Core
{
    /// <summary>
    /// Represents the base class that all entities derive from.
    /// </summary>
    public abstract class Entity<Tkey> where Tkey : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TKey}"/> class.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        protected Entity(Tkey id) : this()
        {
            Ensure.NotNull(id, "The identifier is required.");
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TKey}"/> class.
        /// </summary>
        /// <remarks>
        /// Required by EF Core.
        /// </remarks>
#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。

        protected Entity()
#pragma warning restore CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
        {
        }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        public Tkey Id { get; init; }
    }
}
namespace SharedKernel.Core
{
    /// <summary>
    /// Represents the marker interface for auditable entities.
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// Gets the created on date and time in UTC format.
        /// </summary>

        DateTime CreatedAtUtc { get; }

        /// <summary>
        /// Gets the modified on date and time in UTC format.
        /// </summary>
        DateTime? UpdatedAtUtc { get; }

        /// <summary>
        /// Gets the user who created the entity.
        /// </summary>
        string CreatedBy { get; }

        /// <summary>
        /// Gets the user who last modified the entity.
        /// </summary>
        string? UpdatedBy { get; }
    }
}
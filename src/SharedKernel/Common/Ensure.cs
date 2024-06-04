using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharedKernel.Common
{
    /// <summary>
    /// Contains assertions for the most common application checks.
    /// </summary>
    public static class Ensure
    {
        public static void NotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression("value")] string? paramName = default)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Ensures that the specified <see cref="string"/> value is not empty.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
        public static void NotEmpty([NotNull] string value, string message, [CallerArgumentExpression("value")] string? argumentName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Ensures that the specified <see cref="Guid"/> value is not empty.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
        public static void NotEmpty([NotNull] Guid value, string message, [CallerArgumentExpression("value")] string? argumentName = null)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Ensures that the specified <see cref="DateTime"/> value is not empty.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentException"> if the specified value is the default value for the type.</exception>
        public static void NotEmpty([NotNull] DateTime value, string message, [CallerArgumentExpression("value")] string? argumentName = null)
        {
            if (value == default)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Ensures that the specified value is not null.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentNullException"> if the specified value is null.</exception>
        public static void NotNull<T>(T? value, string message, [CallerArgumentExpression("value")] string? argumentName = null)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(argumentName, message);
            }
        }
    }
}
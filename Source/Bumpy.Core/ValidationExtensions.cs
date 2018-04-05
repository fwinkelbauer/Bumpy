using System;
using System.Diagnostics;

namespace Bumpy.Core
{
    /// <summary>
    /// A set of validation extensions.
    /// </summary>
    [DebuggerStepThrough]
    internal static class ValidationExtensions
    {
        /// <summary>
        /// Throws an ArgumentOutOfRangeException if a specified criteria is not met.
        /// </summary>
        /// <param name="value">The actual value to be checked</param>
        /// <param name="condition">A condition which should apply to the value</param>
        /// <param name="paramName">The name of the value</param>
        /// <param name="message">An error message that should be included if an Exception is thrown</param>
        /// <returns>The specified value</returns>
        public static int ThrowIfOutOfRange(this int value, Func<int, bool> condition, string paramName, string message)
        {
            if (condition(value))
            {
                throw new ArgumentOutOfRangeException(paramName, message);
            }

            return value;
        }
    }
}

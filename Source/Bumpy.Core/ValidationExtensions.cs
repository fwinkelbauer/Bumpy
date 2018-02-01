using System;
using System.Diagnostics;

namespace Bumpy.Core
{
    [DebuggerStepThrough]
    internal static class ValidationExtensions
    {
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

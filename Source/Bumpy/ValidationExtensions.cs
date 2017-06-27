using System;
using System.Diagnostics;

namespace Bumpy
{
    [DebuggerStepThrough]
    public static class ValidationExtensions
    {
        public static T ThrowIfNull<T>([ValidatedNotNull] this T t, string paramName)
            where T : class
        {
            if (t == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (paramName == null)
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            return t;
        }

        public static int ThrowIfCondition(this int value, Func<int, bool> condition, string paramName, string message)
        {
            condition.ThrowIfNull(nameof(condition));
            paramName.ThrowIfNull(nameof(paramName));
            message.ThrowIfNull(nameof(message));

            if (condition(value))
            {
                throw new ArgumentOutOfRangeException(paramName, message);
            }

            return value;
        }
    }
}

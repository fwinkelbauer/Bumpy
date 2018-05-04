using System;

namespace Bumpy.Core.Config
{
    /// <summary>
    /// A custom exception used for all Configuration classes.
    /// </summary>
    [Serializable]
    public sealed class ConfigException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigException"/> class.
        /// </summary>
        /// <param name="message">The Exception message</param>
        public ConfigException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigException"/> class.
        /// </summary>
        /// <param name="message">The Exception message</param>
        /// <param name="innerException">The inner Exception</param>
        public ConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

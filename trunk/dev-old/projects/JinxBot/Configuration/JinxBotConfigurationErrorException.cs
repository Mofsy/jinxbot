using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Configuration
{
    public class JinxBotConfigurationErrorException : Exception
    {
        public JinxBotConfigurationErrorException() { }

        public JinxBotConfigurationErrorException(string message) : base(message) { }

        public JinxBotConfigurationErrorException(string message, Exception inner) : base(message, inner) { }

        internal JinxBotConfigurationErrorException(JinxBotConfigurationError errorKind)
        {
            ErrorCode = errorKind;
        }

        public JinxBotConfigurationError ErrorCode
        {
            get;
            private set;
        }
    }

    public enum JinxBotConfigurationError
    {
        None = 0,
        CorruptConfigurationFile,
    }
}

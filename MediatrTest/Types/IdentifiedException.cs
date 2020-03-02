using System;

namespace MediatrTest.Types
{
    namespace AspNetCore.Extensions.Types
    {
        public class IdentifiedException : Exception
        {
            public string Code { get; }

            public IdentifiedException(string code, string message) : base(FormatMessage(code, message))
            {
                Code = code;
            }

            public IdentifiedException(string code, Exception inner) : base(FormatMessage(code, inner.Message))
            {
                Code = code;
            }

            private static string FormatMessage(string code, string message)
            {
                return $"{message} (Code: {code})";
            }
        }

        public class IdentifiedDomainException : IdentifiedException
        {
            public string Domain { get; }

            public IdentifiedDomainException(string code, string domain, string message) : base(code,
                FormatMessage(domain, message))
            {
                Domain = domain;
            }

            private static string FormatMessage(string domain, string message)
            {
                return $"[{domain}] {message}";
            }
        }
    }
}
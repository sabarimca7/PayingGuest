using System;

namespace PayingGuest.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.") { }
    }

    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new List<string> { message };
        }

        public ValidationException(List<string> errors) : base("Validation failed")
        {
            Errors = errors;
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Unauthorized access") : base(message) { }
    }

    public class TokenException : Exception
    {
        public TokenException(string message) : base(message) { }
    }
}
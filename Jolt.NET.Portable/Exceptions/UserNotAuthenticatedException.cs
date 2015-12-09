using System;

namespace Jolt.NET.Exceptions
{
    public class UserNotAuthenticatedException : Exception
    {
        public UserNotAuthenticatedException(string username, string message = "The user must be authenticated to use the API.")
            : base(message)
        { }
    }
}

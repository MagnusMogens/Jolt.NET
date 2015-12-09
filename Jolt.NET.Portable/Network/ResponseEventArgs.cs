using System;
using Jolt.NET.Data;

namespace Jolt.NET.Network
{
    public class ResponseEventArgs : EventArgs
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public ResponseEventArgs(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }

        public ResponseEventArgs(SuccessResponse response)
        {
            Success = response.Success;
            Message = response.Message;
        }
    }
}

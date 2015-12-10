using System;

namespace Jolt.NET.Network
{
    public class NetworkEventArgs : EventArgs
    {
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }

        public NetworkEventArgs(string url, DateTime? createdAt = null)
        {
            Url = url;
            CreatedAt = createdAt ?? DateTime.Now;
        }
    }
}

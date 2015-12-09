using System.Xml.Serialization;
using Jolt.NET.Base;

namespace Jolt.NET.Data
{
    /// <summary>
    /// The base class of a success response. Can be used standalone.
    /// </summary>
    [XmlRoot("response")]
    public class SuccessResponse : Notifieable
    {
        private bool _success;
        private string _message;
        
        [XmlElement("success")]
        public bool Success
        {
            get { return _success; }
            set { Set(ref _success, value); }
        }

        [XmlElement("message")]
        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }
    }
}

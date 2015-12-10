using System.Xml.Serialization;

namespace Jolt.NET.Data
{
    public enum UserStatus
    {
        [XmlEnum("N/A")]
        Unknown,
        [XmlEnum("Active")]
        Active,
        [XmlEnum("Banned")]
        Banned
    }
}

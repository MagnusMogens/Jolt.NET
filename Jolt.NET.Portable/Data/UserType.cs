using System.Xml.Serialization;

namespace Jolt.NET.Data
{
    public enum UserType
    {
        [XmlEnum("N/A")]
        Unknown,
        [XmlEnum("User")]
        User,
        [XmlEnum("Developer")]
        Developer
    }
}

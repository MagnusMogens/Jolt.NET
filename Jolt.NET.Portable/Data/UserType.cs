using System.Xml.Serialization;

namespace Jolt.NET.Data
{
    public enum UserType
    {
        [XmlEnum("")]
        Unknown,
        [XmlEnum("User")]
        User,
        [XmlEnum("Developer")]
        Developer
    }
}

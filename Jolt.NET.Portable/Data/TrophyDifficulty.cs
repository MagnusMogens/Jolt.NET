using System.Xml.Serialization;

namespace Jolt.NET.Data
{
    public enum TrophyDifficulty
    {
        /// <summary>
        /// An unknown trophy difficulty was delivered.
        /// </summary>
        [XmlEnum("")]
        Unknown,
        [XmlEnum("Bronze")]
        Bronze,
        [XmlEnum("Silver")]
        Silver,
        [XmlEnum("Gold")]
        Gold,
        [XmlEnum("Platinum")]
        Platinum
    }
}

using System.ComponentModel.DataAnnotations;

namespace Jolt.NET.Network
{
    public enum ReturnFormat
    {
        [Display(Description = "")]
        Unknown,
        /// <summary>
        /// This is the default return method.
        /// </summary>
        [Display(Description = "?format=keypair")]
        KeyPair,
        [Display(Description = "?format=json")]
        Json,
        [Display(Description = "?format=xml")]
        Xml,
        [Display(Description = "?format=dump")]
        Dump
    }
}

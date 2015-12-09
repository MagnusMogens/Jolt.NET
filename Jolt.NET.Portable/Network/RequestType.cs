using System.ComponentModel.DataAnnotations;

namespace Jolt.NET.Network
{
    public enum RequestType
    {
        [Display(Description = "users")]
        Users,
        [Display(Description = "sessions")]
        Sessions,
        [Display(Description = "trophies")]
        Trophies,
        [Display(Description = "scores")]
        Scores,
        [Display(Description = "data-store")]
        DataStore
    }
}

using System.ComponentModel.DataAnnotations;

namespace Jolt.NET.Data
{
    public enum SessionStatus
    {
        [Display(Description = "active")]
        Active,
        [Display(Description = "idle")]
        Idle
    }
}

using System.ComponentModel.DataAnnotations;

namespace Jolt.NET.Network
{
    public enum RequestAction
    {
        [Display(Description = "")]
        Nothing,
        [Display(Description = "auth")]
        Auth,
        [Display(Description = "open")]
        Open,
        [Display(Description = "ping")]
        Ping,
        [Display(Description = "close")]
        Close,
        [Display(Description = "add-achieved")]
        AddAchieved,
        [Display(Description = "add")]
        Add,
        [Display(Description = "tables")]
        Tables,
        [Display(Description = "set")]
        Set,
        [Display(Description = "update")]
        Update,
        [Display(Description = "remove")]
        Remove,
        [Display(Description = "get-keys")]
        GetKeys
    }
}

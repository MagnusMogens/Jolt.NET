using System.ComponentModel.DataAnnotations;

namespace Jolt.NET.Network
{
    public enum RequestParameter
    {
        [Display(Description = "")]
        Unknown,
        [Display(Description = "game_id")]
        GameId,
        [Display(Description = "user_id")]
        UserId,
        [Display(Description = "username")]
        Username,
        [Display(Description = "user_token")]
        UserToken,
        [Display(Description = "status")]
        Status,
        [Display(Description = "achieved")]
        Achieved,
        [Display(Description = "trophy_id")]
        TrophyId,
        [Display(Description = "limit")]
        Limit,
        [Display(Description = "table_id")]
        TableId,
        [Display(Description = "score")]
        Score,
        [Display(Description = "sort")]
        Sort,
        [Display(Description = "quest")]
        Guest,
        [Display(Description = "extra_data")]
        ExtraData,
        [Display(Description = "key")]
        Key,
        [Display(Description = "data")]
        Data,
        [Display(Description = "operation")]
        Operation,
        [Display(Description = "value")]
        Value
    }
}

using System.ComponentModel.DataAnnotations;

namespace Jolt.NET.Data.DataStorage
{
    public enum DataStorageUpdateOperation
    {
        [Display(Description = "add")]
        Add,
        [Display(Description = "subtract")]
        Subtract,
        [Display(Description = "multiply")]
        Multiply,
        [Display(Description = "divide")]
        Divide,
        [Display(Description = "append")]
        Append,
        [Display(Description = "prepend")]
        Prepend
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jolt.NET.Core.Interfaces
{
    public interface IStoredData<T>
    {
        T Data { get; set; }
    }
}

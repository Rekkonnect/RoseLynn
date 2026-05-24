using Garyon.Extensions.Comparison;
using Garyon.Objects;
using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RoseLynn;

public sealed class LocationComparer : IComparer<Location>, ISharedInstance
{
    public int Compare(Location? x, Location? y)
    {
        return x.BeginCompare(y)
            .By(static s => s?.SourceTree?.FilePath ?? string.Empty)
            .ThenBy(static s => s?.SourceSpan ?? default)
            .Result
            ;
    }
}

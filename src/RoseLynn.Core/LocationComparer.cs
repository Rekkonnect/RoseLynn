using Garyon.Extensions.Comparison;
using Garyon.Objects;
using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RoseLynn.Core;

/// <summary>Compares <seealso cref="Location"/> instances by file path and source span.</summary>
public sealed class LocationComparer : IComparer<Location>, ISharedInstance
{
    /// <inheritdoc/>
    public int Compare(Location? x, Location? y)
    {
        return x.BeginCompare(y)
            .By(static s => s?.SourceTree?.FilePath ?? string.Empty)
            .ThenBy(static s => s?.SourceSpan ?? default)
            .Result
            ;
    }
}

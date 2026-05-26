using RoseLynn.Utilities;
using System.Reflection;

namespace RoseLynn.CSharp.SignatureViews;

/// <summary>Provides a mutable record implementation of <see cref="IAssemblyIdentitySignatureView"/>.</summary>
public sealed record AssemblyIdentitySignatureView
    : IAssemblyIdentitySignatureView
{
    /// <summary>Gets or sets the simple assembly name.</summary>
    public required string Name { get; set; }
    /// <summary>Gets or sets the assembly version.</summary>
    public required Version Version { get; set; }
    /// <summary>Gets or sets the culture name for the assembly identity.</summary>
    public required string CultureName { get; set; }
    /// <summary>Gets or sets the assembly name flags.</summary>
    public required AssemblyNameFlags Flags { get; set; }
    /// <summary>Gets or sets the assembly content type.</summary>
    public required AssemblyContentType ContentType { get; set; }
    /// <summary>Gets or sets the public key bytes.</summary>
    public required EquatableImmutableArray<byte> PublicKey { get; set; }
    /// <summary>Gets or sets whether the assembly identity is marked retargetable.</summary>
    public required bool IsRetargetable { get; set; }
}

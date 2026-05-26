using RoseLynn.Utilities;
using System.Reflection;

namespace RoseLynn.CSharp.SignatureViews;

/// <summary>Represents the assembly identity fields captured in a signature view.</summary>
public interface IAssemblyIdentitySignatureView
{
    /// <summary>Gets the simple assembly name.</summary>
    public string Name { get; }
    /// <summary>Gets the assembly version.</summary>
    public Version Version { get; }
    /// <summary>Gets the culture name for the assembly identity.</summary>
    public string CultureName { get; }
    /// <summary>Gets the assembly name flags.</summary>
    public AssemblyNameFlags Flags { get; }
    /// <summary>Gets the assembly content type.</summary>
    public AssemblyContentType ContentType { get; }
    /// <summary>Gets the public key bytes.</summary>
    public EquatableImmutableArray<byte> PublicKey { get; }
    /// <summary>Gets whether the assembly identity is marked retargetable.</summary>
    public bool IsRetargetable { get; }
}

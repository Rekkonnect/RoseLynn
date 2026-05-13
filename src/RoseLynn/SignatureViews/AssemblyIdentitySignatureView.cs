using RoseLynn.Utilities;
using System.Reflection;

namespace RoseLynn.SignatureViews;

public sealed record AssemblyIdentitySignatureView
    : IAssemblyIdentitySignatureView
{
    public required string Name { get; set; }
    public required Version Version { get; set; }
    public required string CultureName { get; set; }
    public required AssemblyNameFlags Flags { get; set; }
    public required AssemblyContentType ContentType { get; set; }
    public required EquatableImmutableArray<byte> PublicKey { get; set; }
    public required bool IsRetargetable { get; set; }
}

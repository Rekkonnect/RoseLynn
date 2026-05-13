using RoseLynn.Utilities;
using System.Reflection;

namespace RoseLynn.SignatureViews;

public interface IAssemblyIdentitySignatureView
{
    public string Name { get; }
    public Version Version { get; }
    public string CultureName { get; }
    public AssemblyNameFlags Flags { get; }
    public AssemblyContentType ContentType { get; }
    public EquatableImmutableArray<byte> PublicKey { get; }
    public bool IsRetargetable { get; }
}

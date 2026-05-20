using Garyon.Extensions;

namespace RoseLynn.Writing;

public interface ICSharpImport
{
    public CSharpImportKind Kind { get; }
}

public abstract record CSharpMemberImport(
    string Member,
    string? Alias = null,
    bool Global = false)
    : ICSharpImport
{
    public abstract CSharpImportKind MemberKind { get; }

    public bool HasAlias => Alias is not null;

    public CSharpImportKind Kind
        => MemberKind
        | Global.ValueOrDefault(CSharpImportKind.Global)
        | HasAlias.ValueOrDefault(CSharpImportKind.AliasedImport)
        ;
}

public sealed record CSharpNamespaceImport(
    string Namespace,
    string? Alias = null,
    bool Global = false)
    : CSharpMemberImport(Namespace, Alias, Global)
{
    public override CSharpImportKind MemberKind => CSharpImportKind.Namespace;
}

public sealed record CSharpTypeImport(
    string Type,
    string? Alias = null,
    bool Global = false)
    : CSharpMemberImport(Type, Alias, Global)
{
    public override CSharpImportKind MemberKind => CSharpImportKind.Type;
}

public sealed record CSharpExternAliasImport(string Alias)
    : ICSharpImport
{
    public CSharpImportKind Kind => CSharpImportKind.ExternAlias;
}

public enum CSharpImportKind
{
    None,

    Namespace = 1 << 0,
    Type = 1 << 1,

    AliasedImport = 1 << 3,

    Global = 1 << 6,

    ExternAlias = 1 << 10,
}

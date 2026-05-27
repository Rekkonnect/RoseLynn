#!/usr/bin/dotnet run
#:project ../../src/RoseLynn/RoseLynn.Core.csproj
#:project ../../src/RoseLynn.CSharp/RoseLynn.CSharp.csproj
#:package Garyon
#:package Microsoft.CodeAnalysis.CSharp

using System.Collections.Immutable;
using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.CSharp;

var solutionDirectory = WorkspaceHelpers.GetThisSolutionRoot()
    ?? throw new InvalidOperationException("Could not locate the solution root.");

var srcDirectory = new DirectoryInfo(Path.Combine(solutionDirectory.FullName, "src"));
if (!srcDirectory.Exists)
    throw new DirectoryNotFoundException($"src/ not found under {solutionDirectory.FullName}");

var projectDirectories = srcDirectory
    .GetDirectories()
    .Where(d => d.GetFiles("*.csproj").Length > 0)
    .OrderBy(d => d.Name)
    .ToList();

Console.WriteLine($"Scanning {projectDirectories.Count} project(s) under {srcDirectory.FullName}");
Console.WriteLine();

var allUndocumented = new List<(string Project, string SymbolName)>();

var parseOptions = new CSharpParseOptions(
    languageVersion: LanguageVersion.Default,
    documentationMode: DocumentationMode.Diagnose);

var metadataReferences = GetTrustedPlatformAssemblyReferences();

foreach (var projectDirectory in projectDirectories)
{
    var sourceFiles = projectDirectory
        .GetFiles("*.cs", SearchOption.AllDirectories)
        .Where(f => !IsInObjDirectory(f))
        .ToArray();

    if (sourceFiles.Length == 0)
        continue;

    var syntaxTrees = sourceFiles
        .Select(f => CSharpSyntaxTree.ParseText(
            File.ReadAllText(f.FullName),
            parseOptions,
            f.FullName))
        .ToArray();

    var compilation = CSharpCompilation.Create(
        projectDirectory.Name,
        syntaxTrees,
        metadataReferences,
        new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            nullableContextOptions: NullableContextOptions.Enable));

    var visitor = new UndocumentedMemberVisitor(compilation);
    visitor.Visit(compilation.GlobalNamespace);

    foreach (var name in visitor.UndocumentedMembers)
        allUndocumented.Add((projectDirectory.Name, name));
}

if (allUndocumented.Count == 0)
{
    Console.WriteLine("All public members are documented.");
    return 0;
}

Console.WriteLine($"Found {allUndocumented.Count} undocumented public member(s):");
Console.WriteLine();

foreach (var group in allUndocumented.GroupBy(x => x.Project).OrderBy(g => g.Key))
{
    Console.WriteLine($"[{group.Key}]");
    foreach (var (_, name) in group.OrderBy(x => x.SymbolName))
        Console.WriteLine($"  {name}");
    Console.WriteLine();
}

return 0;

static bool IsInObjDirectory(FileInfo file) =>
    file.FullName.Contains(
        $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
        StringComparison.OrdinalIgnoreCase);

static ImmutableArray<MetadataReference> GetTrustedPlatformAssemblyReferences()
{
    var paths = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
    if (paths is null)
        return [];

    return [.. paths
        .Split(Path.PathSeparator)
        .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))];
}

sealed class UndocumentedMemberVisitor(Compilation compilation) : SymbolVisitor
{
    private readonly List<string> _undocumentedMembers = [];

    public IReadOnlyList<string> UndocumentedMembers => _undocumentedMembers;

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        foreach (var member in symbol.GetMembers())
            member.Accept(this);
    }

    public override void VisitNamedType(INamedTypeSymbol symbol)
    {
        // Extension block types are compiler-synthesised — they cannot carry
        // XML doc comments and are invisible to callers; skip reporting them
        // but still recurse so their members are checked.
        if (IsVisible(symbol) && IsInSource(symbol) && !IsExtensionBlockType(symbol))
            CheckAndRecord(symbol);

        foreach (var member in symbol.GetMembers())
            member.Accept(this);
    }

    public override void VisitMethod(IMethodSymbol symbol)
    {
        if (!IsVisible(symbol) || !IsInSource(symbol))
            return;
        if (symbol.MethodKind
            is MethodKind.PropertyGet
            or MethodKind.PropertySet
            or MethodKind.EventAdd
            or MethodKind.EventRemove
            or MethodKind.StaticConstructor
            or MethodKind.Destructor
            or MethodKind.ExplicitInterfaceImplementation)
            return;
        CheckAndRecord(symbol);
    }

    public override void VisitProperty(IPropertySymbol symbol)
    {
        if (IsVisible(symbol) && IsInSource(symbol))
            CheckAndRecord(symbol);
    }

    public override void VisitField(IFieldSymbol symbol)
    {
        if (IsVisible(symbol) && IsInSource(symbol))
            CheckAndRecord(symbol);
    }

    public override void VisitEvent(IEventSymbol symbol)
    {
        if (IsVisible(symbol) && IsInSource(symbol))
            CheckAndRecord(symbol);
    }

    private static bool IsVisible(ISymbol symbol) =>
        symbol.DeclaredAccessibility
            is Accessibility.Public
            or Accessibility.Protected
            or Accessibility.ProtectedOrInternal;

    private static bool IsInSource(ISymbol symbol) =>
        symbol.Locations.Any(static l => l.IsInSource);

    private static bool IsExtensionBlockType(INamedTypeSymbol symbol)
    {
        var name = symbol.Name;
        return name.Length > 2 && name[0] == '<' && name[^1] == '>';
    }

    private void CheckAndRecord(ISymbol symbol)
    {
        var xml = symbol.GetDocumentationCommentXml();
        if (!string.IsNullOrWhiteSpace(xml))
            return;

        _undocumentedMembers.Add(FormatSymbol(symbol));
    }

    private string FormatSymbol(ISymbol symbol)
    {
        if (symbol is INamedTypeSymbol)
            return $"type {SymbolNameHelpers.FullyQualifiedString(symbol, compilation)}";

        var containerFqn = SymbolNameHelpers.FullyQualifiedString(symbol.ContainingType, compilation);
        var kind = GetMemberKind(symbol);
        var memberDisplay = symbol.ToDisplayString(MemberSignatureFormat);
        return $"in {containerFqn} {kind} {memberDisplay}";
    }

    private static string GetMemberKind(ISymbol symbol) => symbol switch
    {
        IMethodSymbol { MethodKind: MethodKind.Constructor } => "constructor",
        IMethodSymbol { MethodKind: MethodKind.UserDefinedOperator } => "operator",
        IMethodSymbol { MethodKind: MethodKind.Conversion } => "conversion",
        IMethodSymbol => "method",
        IPropertySymbol { IsIndexer: true } => "indexer",
        IPropertySymbol => "property",
        IFieldSymbol => "field",
        IEventSymbol => "event",
        _ => "member",
    };

    private static readonly SymbolDisplayFormat MemberSignatureFormat = new(
        memberOptions:
            SymbolDisplayMemberOptions.IncludeParameters |
            SymbolDisplayMemberOptions.IncludeRef,
        parameterOptions:
            SymbolDisplayParameterOptions.IncludeType |
            SymbolDisplayParameterOptions.IncludeName |
            SymbolDisplayParameterOptions.IncludeParamsRefOut,
        genericsOptions:
            SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
}

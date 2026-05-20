using Garyon.Extensions;
using Garyon.Objects.Strings;
using Microsoft.CodeAnalysis;
using System.Text;

namespace RoseLynn.Writing;

public abstract class CSharpSourceFileBuilder<TCodeWriter>
    where TCodeWriter : CSharpCodeWriter
{
    private readonly HashSet<ICSharpImport> _imports = new();

    private TCodeWriter? _mainContentWriter;
    private TCodeWriter? _dynamicHeaderWriter;

    protected TCodeWriter MainContentWriter
        => _mainContentWriter ??= CreateCodeBuilder();

    protected TCodeWriter DynamicHeaderWriter
        => _dynamicHeaderWriter ??= CreateCodeBuilder();

    protected abstract TCodeWriter CreateCodeBuilder();

    protected abstract string? GetHeader();

    protected abstract string? GetFooter();

    protected abstract void BuildContent();

    protected void AddImport(ICSharpImport import)
    {
        _imports.Add(import);
    }

    protected string FullyQualifiedName(
        ITypeSymbol type,
        Compilation? compilation)
    {
        var result = SymbolNameHelpers.FullyQualified(type, compilation);
        return CaptureFullyQualifiedName(result);
    }

    protected string FullyQualifiedNameOrKeyword(
        ITypeSymbol type,
        Compilation? compilation)
    {
        var result = SymbolNameHelpers.FullyQualifiedOrKeyword(type, compilation);
        return CaptureFullyQualifiedName(result);
    }

    private string CaptureFullyQualifiedName(SymbolNameHelpers.FullyQualifiedName result)
    {
        foreach (var externAlias in result.ExternAliases)
        {
            AddImport(new CSharpExternAliasImport(externAlias));
        }

        return result.Name;
    }

    public string BuildFile()
    {
        var builder = new StringBuilder();

        BuildContent();

        var dynamicHeader = _dynamicHeaderWriter?.ToString();
        var mainContent = _mainContentWriter?.ToString();

        var blockList = new StringBuilderSeparatableBlockList(builder);
        AppendNonNull(GetHeader());
        AppendNonNull(dynamicHeader);
        AppendImports(builder, blockList);
        AppendNonNull(mainContent);
        AppendNonNull(GetFooter());

        return builder.ToString().NormalizeNewLines(WhitespaceFacts.CrLf);

        void AppendNonNull(string? line)
        {
            if (line is not null)
            {
                blockList.BeginNewBlock();
                builder.Append(line);
                blockList.CommitBlock();
            }
        }
    }

    private void AppendImports(
        StringBuilder builder,
        StringBuilderSeparatableBlockList separatableBlockList)
    {
        if (_imports.Count is 0)
        {
            return;
        }

        const CSharpImportKind segmentKindMask
            = CSharpImportKind.ExternAlias
            | CSharpImportKind.Global
            ;

        separatableBlockList.BeginNewBlock();

        // Segmentation is necessary due to language rules
        var segmentGroups = _imports
            .GroupBy(ImportSegmentKind)
            .ToListDictionary();

        var hasPreviousSegment = false;
        foreach (var group in segmentGroups)
        {
            var imports = group.Value;
            if (hasPreviousSegment)
            {
                builder.AppendLine();
            }

            // TODO: Sort the imports for extra fanciness, although
            // highly advised against in generated sources due to conflicts
            foreach (var import in imports)
            {
                builder.AppendLine(GetImportText(import));
            }

            hasPreviousSegment = true;
        }

        return;

        static CSharpImportKind ImportSegmentKind(ICSharpImport s) => s.Kind & segmentKindMask;
    }

    private static string GetImportText(ICSharpImport import)
    {
        return import switch
        {
            CSharpMemberImport member => MemberImportText(member),
            CSharpExternAliasImport aliasImport => ExternAliasImportText(aliasImport),
            _ => throw new InvalidOperationException("Unsupported import type."),
        };

        static string MemberImportText(CSharpMemberImport import)
        {
            var aliasPart = import.Alias is not null ? $"{import.Alias} = " : null;
            var globalPart = import.Global ? "global " : null;
            return $"{globalPart}using {aliasPart}{import.Member};";
        }

        static string ExternAliasImportText(CSharpExternAliasImport aliasImport)
        {
            return $"extern alias {aliasImport.Alias}";
        }
    }
}

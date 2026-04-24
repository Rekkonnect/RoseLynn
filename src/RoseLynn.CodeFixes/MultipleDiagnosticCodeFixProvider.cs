using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using RoseLynn.Analyzers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.CodeFixes;

/// <summary>Provides mechanisms to better organize registering code fixes to the environment.</summary>
public abstract class MultipleDiagnosticCodeFixProvider : CodeFixProvider
{
    private readonly ImmutableArray<string> fixableDiagnosticIds;

    /// <summary>The <seealso cref="DiagnosticDescriptor"/>s this code fix is triggered on.</summary>
    /// <remarks>Using these instances is preferred over typing out the raw IDs themselves, since they're more accessible with the help of <seealso cref="DiagnosticDescriptorStorageBase"/>.</remarks>
    protected abstract IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors { get; }
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => fixableDiagnosticIds;

    private string codeFixTitle;
    /// <summary>The title of the code fix. By default, it looks up for a resource string named "{TypeName}_Title" with TypeName being the name of the type.</summary>
    public virtual string CodeFixTitle => codeFixTitle ??= ResourceManager.GetString($"{GetType().Name}_Title");

    /// <summary>Gets the <seealso cref="DiagnosticDescriptorStorageBase"/> that contains this code fix's title.</summary>
    protected abstract ResourceManager ResourceManager { get; }

    /// <summary>Initializes a new <seealso cref="MultipleDiagnosticCodeFixProvider"/> instance, discovering its <seealso cref="FixableDiagnosticIds"/>.</summary>
    protected MultipleDiagnosticCodeFixProvider()
    {
        fixableDiagnosticIds = FixableDiagnosticDescriptors.Select(d => d.Id).ToImmutableArray();
    }

    /// <summary>Provides the <seealso cref="FixAllProvider"/> for this code fix. Defaults to <seealso cref="WellKnownFixAllProviders.BatchFixer"/>. Returning <see langword="null"/> indicates that the code fix does not support fixing multiple occurrences.</summary>
    /// <returns>The default <seealso cref="FixAllProvider"/>, <seealso cref="WellKnownFixAllProviders.BatchFixer"/>.</returns>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.GetSyntaxRootAsync();

        foreach (var diagnostic in context.Diagnostics)
        {
            var codeAction = CodeAction.Create(CodeFixTitle, PerformAction, GetType().Name);
            context.RegisterCodeFix(codeAction, diagnostic);

            Task<Document> PerformAction(CancellationToken cancellationToken)
            {
                var syntaxNode = root.FindNode(diagnostic.Location.SourceSpan);
                return PerformCodeFixActionAsync(context, syntaxNode, cancellationToken);
            }
        }
    }

    /// <summary>Performs the code fix action, given the <seealso cref="SyntaxNode"/> the diagnostic is reported on.</summary>
    /// <param name="context">The <seealso cref="CodeFixContext"/>.</param>
    /// <param name="syntaxNode">The <seealso cref="SyntaxNode"/> on which the diagnostic triggering this code fix is reported on.</param>
    /// <param name="cancellationToken">The <seealso cref="CancellationToken"/>.</param>
    /// <returns>The resulting <seealso cref="Document"/> after performing the code fix.</returns>
    protected abstract Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken);
}

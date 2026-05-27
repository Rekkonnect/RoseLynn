using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn;

/// <summary>Provides workspace-level helpers for aggregating syntax roots across projects and solutions.</summary>
public static class WorkspaceExtensions
{
    /// <summary>Gets the syntax roots of all documents in every project of <paramref name="solution"/>.</summary>
    /// <param name="solution">The solution whose document roots to collect.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of non-null syntax roots from all documents.</returns>
    public static async Task<IReadOnlyCollection<SyntaxNode>> GetSyntaxRootsInSolution(
        this Solution solution, CancellationToken cancellationToken)
    {
        return await solution.Projects
            .SelectMany(s => s.Documents)
            .GetAllNotNullNodes(cancellationToken)
            ;
    }

    /// <summary>Gets the syntax roots of all documents in <paramref name="project"/>.</summary>
    /// <param name="project">The project whose document roots to collect.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of non-null syntax roots from the project's documents.</returns>
    public static async Task<IReadOnlyCollection<SyntaxNode>> GetSyntaxRootsInProject(
        this Project project, CancellationToken cancellationToken)
    {
        return await project.Documents
            .GetAllNotNullNodes(cancellationToken);
    }
}

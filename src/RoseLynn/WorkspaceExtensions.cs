using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn;

public static class WorkspaceExtensions
{
    public static async Task<IReadOnlyCollection<SyntaxNode>> GetSyntaxRootsInSolution(
        this Solution solution, CancellationToken cancellationToken)
    {
        return await solution.Projects
            .SelectMany(s => s.Documents)
            .GetAllNotNullNodes(cancellationToken)
            ;
    }

    public static async Task<IReadOnlyCollection<SyntaxNode>> GetSyntaxRootsInProject(
        this Project project, CancellationToken cancellationToken)
    {
        return await project.Documents
            .GetAllNotNullNodes(cancellationToken);
    }
}

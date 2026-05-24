using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;

namespace RoseLynn.Workspaces;

public static class CodeRefactoringContextExtensions
{
    extension(CodeRefactoringContext context)
    {
        public async Task<SyntaxNode?> GetSyntaxNodeAsync()
        {
            return await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        }

        public Solution Solution => context.Document.Solution;
    }
}

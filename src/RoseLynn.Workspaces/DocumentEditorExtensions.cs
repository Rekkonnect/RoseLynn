using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace RoseLynn.Workspaces;

public static class DocumentEditorExtensions
{
    extension(DocumentEditor editor)
    {
        public DocumentId DocumentId => editor.OriginalDocument.Id;

        public DocumentEditor AnnotateNode(SyntaxNode node, SyntaxAnnotation annotation)
        {
            editor.ReplaceNode(node, node.WithAdditionalAnnotations(annotation));
            return editor;
        }

        public DocumentEditor AnnotateNodes(
            IReadOnlyCollection<SyntaxNode> nodes,
            string annotationKind,
            string annotationValue)
        {
            foreach (var node in nodes)
            {
                editor.AnnotateNode(node, new SyntaxAnnotation(annotationKind, annotationValue));
            }
            return editor;
        }

        public DocumentEditor ReplaceNodes(
            IEnumerable<SyntaxNode> oldNodes,
            Func<SyntaxNode, SyntaxNode> computeReplacementNode)
        {
            foreach (var oldNode in oldNodes)
            {
                editor.ReplaceNode(oldNode, computeReplacementNode(oldNode));
            }
            return editor;
        }
    }
}

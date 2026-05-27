using Microsoft.CodeAnalysis;

namespace RoseLynn.Semantic;

/// <summary>Pairs a <see cref="SyntaxNode"/> with the <see cref="SemanticModel"/> that covers it.</summary>
/// <param name="Node">The syntax node.</param>
/// <param name="Model">The semantic model covering <paramref name="Node"/>.</param>
public readonly record struct NodeWithSemanticModel(SyntaxNode? Node, SemanticModel? Model);

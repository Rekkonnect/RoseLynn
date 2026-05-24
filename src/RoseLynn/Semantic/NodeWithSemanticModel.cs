using Microsoft.CodeAnalysis;

namespace RoseLynn.Semantic;

public readonly record struct NodeWithSemanticModel(SyntaxNode? Node, SemanticModel? Model);

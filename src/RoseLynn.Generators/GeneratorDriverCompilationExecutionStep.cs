using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Captures the input and output state of a generator driver execution step.</summary>
/// <param name="OriginalDriver">The generator driver before execution.</param>
/// <param name="OriginalCompilation">The compilation provided to the generator driver.</param>
/// <param name="ResultingDriver">The generator driver after execution completes.</param>
/// <param name="ResultingCompilation">The compilation produced by the generator driver.</param>
public sealed record class GeneratorDriverCompilationExecutionStep(
    GeneratorDriver OriginalDriver,
    Compilation OriginalCompilation,
    GeneratorDriver ResultingDriver,
    Compilation ResultingCompilation
)
{
    /// <summary>Returns the resulting compilation without any generated syntax trees.</summary>
    /// <returns>A copy of <see cref="ResultingCompilation"/> with generated files removed.</returns>
    public Compilation CleanGeneratedFiles()
    {
        return ResultingCompilation.RemoveGeneratedTrees(ResultingDriver);
    }
}

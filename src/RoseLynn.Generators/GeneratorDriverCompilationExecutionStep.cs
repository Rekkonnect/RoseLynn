using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

[NeedsDocs]
public sealed record class GeneratorDriverCompilationExecutionStep(
    GeneratorDriver OriginalDriver,
    Compilation OriginalCompilation,
    GeneratorDriver ResultingDriver,
    Compilation ResultingCompilation
)
{
    public Compilation CleanGeneratedFiles()
    {
        return ResultingCompilation.RemoveGeneratedTrees(ResultingDriver);
    }
}

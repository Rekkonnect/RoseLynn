using Microsoft.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.Testing.Benchmarks;

[NeedsDocs]
public abstract class BaseGeneratorBenchmarkCompilationProvider
{
    public abstract ValueTask<Compilation> InitializeCompilation(
        Compilation compilation,
        CancellationToken cancellationToken);

    public abstract ValueTask<Compilation> ModifyCompilation(
        int iterationIndex,
        Compilation compilation,
        CancellationToken cancellationToken);
}

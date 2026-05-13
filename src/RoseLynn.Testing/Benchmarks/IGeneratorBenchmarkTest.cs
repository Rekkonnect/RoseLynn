using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.Testing.Benchmarks;

[NeedsDocs]
public interface IGeneratorBenchmarkTest
{
    public Task<GeneratorBenchmarkRunResults> RunBenchmark(
        GeneratorBenchmarkParameters parameters,
        CancellationToken cancellationToken);
}

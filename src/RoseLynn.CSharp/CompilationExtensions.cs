using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn.CSharp;

/// <summary>Contains C#-specific extensions for <seealso cref="Compilation"/> and <seealso cref="CompilationOptions"/>.</summary>
public static class CompilationExtensions
{
    extension(CompilationOptions compilationOptions)
    {
        /// <summary>
        /// Returns a copy of the compilation options with the nullable context options set,
        /// if the options belong to a C# compilation. Otherwise returns the options unchanged.
        /// </summary>
        public CompilationOptions WithNullableContextOptions(
            NullableContextOptions nullableContextOptions)
        {
            if (compilationOptions is CSharpCompilationOptions csharpCompilationOptions)
            {
                return csharpCompilationOptions.WithNullableContextOptions(nullableContextOptions);
            }

            return compilationOptions;
        }
    }

    extension<TCompilation>(TCompilation compilation)
        where TCompilation : Compilation
    {
        /// <summary>
        /// Returns a copy of the compilation with the nullable context options set,
        /// if it is a C# compilation. Otherwise returns the compilation unchanged.
        /// </summary>
        public TCompilation WithNullableContextOptions(
            NullableContextOptions nullableContextOptions)
        {
            return (TCompilation)compilation.WithOptions(
                compilation.Options
                    .WithNullableContextOptions(nullableContextOptions));
        }
    }
}

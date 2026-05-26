using RoseLynn.CSharp;
using System.Runtime.Loader;

namespace RoseLynn.Test;

/// <summary>Tests usings-provider output generation.</summary>
public class UsingsProviderTests
{
    [Test]
    public async Task ClassicUsings()
    {
        const string classicUsings = """
                                     using System;
                                     using System.Collections.Generic;
                                     using System.Linq;
                                     using System.Text;
                                     using System.Threading.Tasks;
                                     """;

        const string exampleCode = """
                                   namespace Example;

                                   public class C
                                   {
                                       public static void Main()
                                       {
                                           Console.WriteLine("It's a beautiful day outside...");
                                       }
                                   }
                                   """;

        var usingsProvider = new VariableUsingsProvider(classicUsings);
        var withUsings = usingsProvider.WithUsings(exampleCode).ReplaceLineEndings();
        var expected = $"{classicUsings}\n{exampleCode}".ReplaceLineEndings();
        await Assert.That(withUsings).IsEqualTo(expected);
    }
}

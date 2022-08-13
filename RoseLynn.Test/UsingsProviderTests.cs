using NUnit.Framework;

namespace RoseLynn.Test;

public class UsingsProviderTests
{
    [Test]
    public void ClassicUsings()
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
        Assert.AreEqual(expected, withUsings);
    }
}

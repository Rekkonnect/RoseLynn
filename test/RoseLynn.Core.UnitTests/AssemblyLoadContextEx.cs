using System.Runtime.Loader;

namespace RoseLynn.Test;

public static class AssemblyLoadContextEx
{
    public static IDisposable CreateDisposableContext(out AssemblyLoadContext context)
    {
        context = new AssemblyLoadContext(null, true);
        return new DisposableContext(context);
    }

    private sealed record DisposableContext(AssemblyLoadContext Context)
        : IDisposable
    {
        public void Dispose()
        {
            Context.Unload();
        }
    }
}

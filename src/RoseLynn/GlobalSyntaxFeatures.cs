namespace RoseLynn.Core;

/// <summary>Represents the set of global C# syntax features present in a compilation.</summary>
[Flags]
public enum GlobalSyntaxFeatures
{
    /// <summary>No global syntax features.</summary>
    None = 0,
    /// <summary>The compilation contains <see langword="global using"/> directives.</summary>
    GlobalUsings = 1 << 0,
    /// <summary>The compilation contains assembly-level attribute declarations.</summary>
    AssemblyAttributes = 1 << 1,
    /// <summary>The compilation contains module-level attribute declarations.</summary>
    ModuleAttributes = 1 << 2,
    /// <summary>The compilation contains top-level statements (file-level program entry point).</summary>
    FileProgram = 1 << 3,
}

namespace RoseLynn;

public enum GlobalSyntaxFeatures
{
    None = 0,
    GlobalUsings = 1 << 0,
    AssemblyAttributes = 1 << 1,
    ModuleAttributes = 1 << 2,
    FileProgram = 1 << 3,
}

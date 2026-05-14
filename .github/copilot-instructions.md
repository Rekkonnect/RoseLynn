# Copilot Instructions

## Repository instructions
- Use XML documentation on public or protected APIs when the intent, contract, or behavior is not immediately obvious.
- Use Roslyn/MCP symbol-based discovery for repo-wide searches for members marked with `NeedsDocs` (avoid bursty command-line scans). Perform a full repo-wide symbol search to find all matching members and add concise XML documentation for each marked member and any directly related child members that would otherwise lack useful context. After adding XML documentation, remove the `NeedsDocs` attribute from those declarations and continue documenting only the remaining attributed declarations that still lack docs.
- Keep documentation focused on intent, inputs, outputs, and notable behavior; do not restate trivial implementation details.
- Avoid repeating the same wording across a type and its members unless the repetition is necessary for clarity

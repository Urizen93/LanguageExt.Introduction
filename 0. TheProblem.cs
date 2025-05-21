using JetBrains.Annotations;
using LanguageExt.Introduction.Examples;

namespace LanguageExt.Introduction;

[PublicAPI]
public static class TheProblem
{
    // Conventional OOP uses exceptions for flow control
    // This leads to several issues:
    /// 1. Signatures provide no feedback <see cref="IThrowingApi"/>
    /// 2. Try-catch on every level <see cref="TryCatches"/>
    /// 3. No compile-time safety <see cref="CompilerFailsToWarn"/>
    public static void ExceptionsAsControlFlowTool()
    {
        // If exceptions are a bad tool to control the flow,
        // which alternatives do we have?
    }
}
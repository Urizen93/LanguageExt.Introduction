using JetBrains.Annotations;
using LanguageExt.Introduction.Examples;

namespace LanguageExt.Introduction;

[PublicAPI]
public static class TheProblem
{
    // Conventional OOP uses exception flow control
    // This leads to several issues:
    /// 1. Signatures provide no feedback <see cref="IThrowingApi"/>
    /// 2. Try-catch on every level <see cref="TryCatches"/>
    public static void ExceptionsAsControlFlowTool()
    {
        
    }
}
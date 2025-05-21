using JetBrains.Annotations;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public static class CompilerFailsToWarn
{
    public static bool TryCheck(int value, out string? result)
    {
        try
        {
            result = PotentiallyThrowingMethod(value);
            return true;
        }
        catch (ArgumentException)
        {
            result = null;
            return false;
        }
    }

    public static string PotentiallyThrowingMethod(int value)
    {
        // If we changed the exception type or add new exception,
        // we'd have to visit all the call sites to handle this (and the call sites of those call sites, and so on!)
        // And if we fail to do so, compiler won't be able to do anything about it, and runtime exceptions will follow
        throw new InvalidOperationException();
    }
}
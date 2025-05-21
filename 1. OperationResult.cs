using JetBrains.Annotations;
using LanguageExt.Introduction.Examples;

namespace LanguageExt.Introduction;

[PublicAPI]
public static class OperationResultAttempt
{
    // Many projects end up inventing some sort of Result container type
    // which they use to communicate potential failures
    public static OperationResult<int> ResultClassAsCommonAttemptToTackleTheProblem()
    {
        // While their hearth is at the right place,
        // they usually lack background in the area, only making it worse
        // Behold a typical specimen:
        return new OperationResult<int>(
            isSuccess: true,
            error: "",
            value: 1);
    }

    /// Let's take a look at what's exactly wrong with it <see cref="CustomResultTypes" />
    public static void FailsToAchieveItsGoals()
    {
        // The idea behind it is correct, though. If only there was a type that actually does all of this...
    }
}
using System.Diagnostics;
using LanguageExt.Common;
using LanguageExt.Introduction.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace LanguageExt.Introduction;

public sealed class EitherNuances : CanPrintOutput
{
    public EitherNuances(ITestOutputHelper output) : base(output)
    {
    }

    [Theory,
     InlineData(1, 1),
     InlineData(1, 2)]
    public void Equality(int firstValue, int secondValue)
    {
        Either<string, int> first = firstValue;
        Either<string, int> second = secondValue;
        
        var stopwatch = Stopwatch.StartNew();
        // Either supports equality but it comes at a price
        var areEqual = first.Equals(second);
        stopwatch.Stop();
        
        WriteLine($"Values {firstValue} and {secondValue} are{(areEqual ? "" : " not")} equal");
        WriteLine($"It took {stopwatch.Elapsed} time to compute");
        
        // Takes half a second on my machine during the first execution
        // The reasons a complicated, the short answer is: don't use default Equals on Either
        
        // If you actually want to check for equality, the simplest way will be to use the matching:
        areEqual = first.Match(
            firstNumber => second.Match(
                secondNumber => firstNumber == secondNumber,
                _ => false),
            firstString => second.Match(
                _ => false,
                secondString => firstString == secondString));
        WriteLine(areEqual);
        // ...yes, far from pretty. The next major update will most likely change the equality mechanism, though
    }

    [Fact]
    public void BottomState()
    {
        // In fact, either has a third, bottom state
        Either<string, int> either = default; // Structs, as good as they are, have their price too :(
        WriteLine(either);

        // Filtering is a thing, and filtered out Either will end up in Bottom state
        either = 1;
        either = either.Where(number => number > 10);
        WriteLine(either);
        
        // Any attempt to collapse it will throw
        Assert.Throws<BottomException>(() => either.IfLeft(1));
        
        // Chains should still work, however: this way a single Where will not break the entire pipeline
        either = either.Select(x => x + x);
        WriteLine(either); // Still bottom

        // Conclusions:
        // 1. Never create a default either
        // 2. If you ever filter, be aware of what you are doing.
        //    You must check for Bottom state RIGHT AFTER the pipe which had a filter in there,
        //    and never return an Either in Bottom state
    }
}
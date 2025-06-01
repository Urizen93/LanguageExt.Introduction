using LanguageExt.Introduction.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace LanguageExt.Introduction;

public sealed class EitherAsyncOverview : CanPrintOutput
{
    public EitherAsyncOverview(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task Overview()
    {
        // There is also EitherAsync type
        // It is basically Task<Either<TLeft, TRight> with some additional flavour

        EitherAsync<string, int> asyncResult = 1;
        EitherAsync<string, int> _ = Task.FromResult(1); // Convertible from Task too!
        // Can be easily converted to Task<Either<...>>
        Task<Either<string, int>> taskEither = asyncResult.ToEither();
        // ... and back
        asyncResult = taskEither.ToAsync(); // this method is particularly useful by the way

        // Can be awaited
        Either<string, int> __ = await asyncResult;
        
        // Cannot be printed as easily, though
        WriteLine(asyncResult);
        
        // Has the same Mapping/Binding/Collapsing capabilities
    }
}
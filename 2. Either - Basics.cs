using System.Net.Mail;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using LanguageExt.Common;
using LanguageExt.Introduction.Services;
using LanguageExt.Introduction.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;
using HttpNotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;
using NotFound = LanguageExt.Introduction.Models.NotFound;

namespace LanguageExt.Introduction;

public sealed class EitherBasics : CanPrintOutput
{
    public EitherBasics(ITestOutputHelper output) : base(output)
    {
    }

    // In fact, it exists for many-many years
    // It has different names in different languages and has strong mathematical background behind it
    // Today, we'll take a look at LanguageExt.Either class
    [Fact]
    public void Intro()
    {
        // It's a container type which is in one of two states:
        // either left and right (there is more to it, we will ignore it for now for the sake of simplicity)
        Either<string, int> stringOrInt = 42;
        WriteLine(stringOrInt);
        
        stringOrInt = "ABC"; // Implicit converters out of the box
        WriteLine(stringOrInt);
        
        // Type safe: the following line does not compile
        // eitherStringOrInt = Guid.NewGuid();
        
        // Has several static constructors
        _ = Prelude.Right<string, int>(123);
        _ = Either<string, int>.Left("You get the idea");
        // Builder-like approach, Bind specifies Left type arg in this case
        Either<string, int> __ = Prelude.Right(1).Bind<string>();
    }

    // A very common use-case: representing a result of an operation which is either (!) error or success
    [Theory,
     ResultData(true),
     ResultData(false)]
    public void RepresentingErrorOrSuccessOperationResult(IReturnEither service, int clientId)
    {
        Either<NotFound, MailAddress> result = service.GetEmail(clientId);
        WriteLine(result);
        
        // Restricts access to the underlying value
        // To get the value, you have to match it first!
        var stringResult = result.Match(
            email => $"Client {clientId} email is {email.Address}", // Note that the right value is matched first
            _ => "No such client exist!");
        WriteLine(stringResult);
        
        // If we are trying to represent some sort of error/failure state,
        // there is a custom that a right value is "success" and a left one is "error" (in broad terms)
        // That's why we matched the right value first
        
        // More realistic example: http results
        _ = result.Match<IActionResult>(
            email => Ok(email.Address),
            _ => NotFound());
        
        // Advice: use type-safe result, and match in a type safe way!
        _ = result.Match<Results<Ok<string>, HttpNotFound>>(
            email => TypedResults.Ok(email.Address),
            FromNotFound);
        
        static Results<Ok<string>, HttpNotFound> FromNotFound(NotFound _) => TypedResults.NotFound();
        
        // There is a very important property of Either - it cannot hold nulls (even if the type is Nullable<T>)!
        Assert.Throws<ValueIsNullException>(() => (Either<string, int?>) (int?) null);
        Assert.Throws<ValueIsNullException>(() => (Either<string?, int?>) (string?) null);
    }

    [Theory,
     InlineData("42"),
     InlineData("something else")]
    public async Task WaysToCollapseTheContext(string input)
    {
        // Extracting a value from Either is called a collapse of the context
        // As a rule of thumb, try to keep the context as long as you can
        // We'll see how you achieve that a bit later

        static Either<Error, int> ParseNumber(string value) =>
            int.TryParse(value, out var result)
                ? result
                : Error.New($"Value is not number, it is \"{value}\"");
            
        Either<Error, int> result = ParseNumber(input);
        
        // Match is one of the ways to collapse the context
        // Either not only cannot hold nulls, it cannot collapse to nulls as well
        Assert.Throws<ResultIsNullException>(() => result.Match<string?>(_ => null, _ => null));
        
        // However, most of the collapsing methods have Unsafe versions.
        // Try to use it only on the borders of an application!
        var unsafeResult = result.MatchUnsafe<string?>(_ => null, _ => null);
        Assert.Null(unsafeResult);

        // Non-returning match overload, a.k.a Switch
        result.Match(
            number => WriteLine($"The number is {number}"),
            error => WriteLine($"Unable to parse a number: {error}"));

        // IfLeft - a way to provide a fallback value. Has IfRight sibling with similar behavior
        int withFallback = result.IfLeft(-1);
        WriteLine($"Value or fallback: {withFallback}");
        
        // IfLeft and IfRight also have overloads for conditional access
        result.IfRight(number => WriteLine($"Executed only in Right state, the number is {number}"));

        // Most methods also have async overloads!
        var asyncCallResult = await result.MatchAsync(MakeApiRequest, _ => Guid.Empty);
        WriteLine(asyncCallResult);

        // There are also less relevant methods like Do, Iter, etc.

        // As you can see, a consumer has no way of "forgetting" to check a "bad" result
        // With that, our first concern is addressed
    }

    #region Irrelevant

    private static Task<Guid> MakeApiRequest(int value) => Task.FromResult(Guid.NewGuid());

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    private sealed class ResultDataAttribute : AutoDataAttribute
    {
        public ResultDataAttribute(bool isSuccess)
            : base(() => CreateFixture(isSuccess))
        {
        }

        private static IFixture CreateFixture(bool isSuccess)
        {
            var fixture = new Fixture()
                .Customize(new AutoNSubstituteCustomization());

            fixture.Freeze<IReturnEither>()
                .GetEmail(Arg.Any<int>())
                .Returns(isSuccess
                    ? fixture.Create<MailAddress>()
                    : new NotFound());

            return fixture;
        }
    }

    #endregion
}
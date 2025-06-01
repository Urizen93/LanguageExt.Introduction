namespace LanguageExt.Introduction;

using JetBrains.Annotations;
using OneOf;
using OneOf.Types;
using System.Net.Mail;
using Utilities;
using Xunit;
using Xunit.Abstractions;

public sealed class EitherAndOneOf : CanPrintOutput
{
    public EitherAndOneOf(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    // There is another quite popular library that represents a similar concept - OneOf
    public void Overview()
    {
        // One of represents a type that could be one of several things
        // When it's just two things, it's basically the same as Either, though it lacks a lot of QoL features
        OneOf<string, int> stringOrInt = 1;
        WriteLine(stringOrInt); // Note that the output differs from Either

        // Does not provide null-safety - use nullable reference types!
        _ = OneOf<string, int>.FromT0(null!); // Does not throw
        
        // Can be collapsed with Match, Switch, or TryPickT0 (and so on)
        WriteLine(
            stringOrInt.TryPickT1(out var number, out var stringValue)
                ? $"Picked int {number}"
                : $"Picked string {stringValue}");
        // Can be mapped with MapT0, MapT1, ...
        // Is not bindable by default - my suggestion is to implement SelectMany/Bind if needed
        
        // Has object Value property, as well as AsT0, AsT1, ... - avoid using those
        Assert.Throws<InvalidOperationException>(() => stringOrInt.AsT0); // Throws if in T1 state
        
        // IsT0, IsT1, ... however are fine if you want to short-circuit
        if (stringOrInt.IsT0) Assert.Fail("Is not supposed to be T0!");
        
        // Can hold more than two cases (up to 8 in base lib and 31 in .Extended)
        OneOf<NotFound, int, string> oneOf3 = "One of those";
        WriteLine(
            oneOf3.Match(
                _ => "Not found",
                code => $"Code {code}",
                text => $"Message {text}"));
        
        // Supports equality, even though it's not IEquatable
        OneOf<string, int> anotherStringOrInt = 1;
        Assert.True(stringOrInt.Equals(anotherStringOrInt));
        
        // Does not have equality with with it's members, the following warning is righteous
        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.False(stringOrInt.Equals(1));
    }
}

// Consider using OneOfBase if you can give a meaningful name or there are just to many arguments
// Has library for source-generation to reduce boilerplate - you'd need to define ctor and conversions otherwise
[PublicAPI, GenerateOneOf]
public sealed partial class GetEmailResult : OneOfBase<NotFound, NoAccess, MailAddress>;

public readonly record struct NoAccess;
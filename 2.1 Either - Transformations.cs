using System.Net.Mail;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using LanguageExt.Common;
using LanguageExt.Introduction.Models;
using LanguageExt.Introduction.Services;
using LanguageExt.Introduction.Utilities;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace LanguageExt.Introduction;

public sealed class EitherTransformations : CanPrintOutput
{
    public EitherTransformations(ITestOutputHelper output) : base(output)
    {
    }

    [Theory, InlineData(100), InlineData(null)]
    public void Transformations(int? value)
    {
        Either<NotFound, int> result = BuildEither(value);
        WriteLine(result);

        // Transform Right value with Select/Map methods
        // If Either is in Left state, the lambda will not execute at all
        Either<NotFound, string> dividedResult = result.Select(number =>
        {
            WriteLine($"Dividing {number}...");
            return $"Divided value is {number / 5}";
        });
        WriteLine(dividedResult);
        
        // This allows us to focus on our current logic,
        // without having to worry about errors that might have occured levels below
        // Take note that we never discard the errors! We just leave them for somebody else to handle later down the line
        
        // Left values can also be mapped with MapLeft. BiMap is also there!
        Either<string, int> mappedLeftResult = result.MapLeft(_ => "The number is missing!");
        WriteLine(mappedLeftResult);
        
        // This allows as to easily transform (map) our values, addressing our second concern 
    }

    [Theory,
     EmailSenderData(doesEmailExist: true, isAbleToSend: true),
     EmailSenderData(doesEmailExist: true, isAbleToSend: false),
     EmailSenderData(doesEmailExist: false, isAbleToSend: true),
     EmailSenderData(doesEmailExist: false, isAbleToSend: false)]
    public void Compositions(IEmailProvider emailProvider, ISendNotifications notificationService, int clientId)
    {
        // Let's try composing some calls
        // Let's say we need to get an email by ID and then send a notification to it
        Either<Error, Guid> notificationResult = emailProvider
            .GetEmail(clientId)
            .Bind(notificationService.SendNotification); // A.k.a SelectMany
        WriteLine($"Trying to send notification to client {clientId}: {notificationResult}");

        // The important thing is that SendNotification is only executed if we actually got an email - exactly what we want!
        WriteLine(notificationService.ReceivedCalls().Select(call => call.GetArguments()).ToArr() switch
        {
            [] => "Notification service did not receive any calls!",
            [[MailAddress mail]] => $"Notification service received a call with the following arg {mail.Address}",
            _ => throw new Exception("Should never happen!")
        });
    }

    [Theory,
     InlineData(1, 2),
     InlineData(1, null),
     InlineData(null, 2),
     InlineData(null, null)]
    public void CompositionsWithLinqExpressions(int? first, int? second)
    {
        Either<NotFound, int> firstEither = BuildEither(first);
        WriteLine($"The first is {firstEither}");
        
        Either<NotFound, int> secondEither = BuildEither(second);
        WriteLine($"The second is {secondEither}");
        
        // Linq-expressions allow us to unbox values into the bounded context easily
        Either<NotFound, int> sum =
            from firstValue in firstEither
            from secondValue in secondEither
            select firstValue + secondValue;
        WriteLine($"The sum is {sum}");
        
        // It can be done with regular linq syntax as well but is more cumbersome (you'll need to pass around value tuples)
    }

    #region Irrelevant

    private static Either<NotFound, int> BuildEither(int? value) => Prelude
        .Optional(value) // Build Option from a nullable type 
        .ToEither(new NotFound()); // Create Either form said Option
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    private sealed class EmailSenderDataAttribute : AutoDataAttribute
    {
        public EmailSenderDataAttribute(bool doesEmailExist, bool isAbleToSend)
            : base(() => CreateFixture(doesEmailExist, isAbleToSend))
        {
        }

        private static IFixture CreateFixture(bool doesEmailExist, bool isAbleToSend)
        {
            var fixture = new Fixture()
                .Customize(new AutoNSubstituteCustomization());

            fixture.Freeze<IEmailProvider>()
                .GetEmail(Arg.Any<int>())
                .Returns(doesEmailExist ? fixture.Create<MailAddress>() : Error.New("Email is not found!"));
                
            fixture.Freeze<ISendNotifications>()
                .SendNotification(Arg.Any<MailAddress>())
                .Returns(isAbleToSend ? Guid.NewGuid() : Error.New("Unable to send notification!"));

            return fixture;
        }
    }

    #endregion
}
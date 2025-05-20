using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using NSubstitute;
using Xunit.Abstractions;
using LanguageExt.Introduction.Utilities;
using Xunit;

namespace LanguageExt.Introduction;

public sealed class OptionExamples
{
    private static void Intro()
    {
        // Basically nullable value with compile-time safety
        // Has two states: Some (with non-null value) and None (without any value)
        // Values inside cannot be null (even if it's Nullable<T>)
        Option<string> maybeString = Prelude.None;

        Option<Guid> someGuid = Prelude.Some(Guid.NewGuid());

        int? nullableInt = null;
        Option<int> maybeInt = Prelude.Optional(nullableInt);
        
        // Advantages
        // 1. No NullReferenceException
        // 2. Clear communication of intent
        // 3. Ease of chaining
    }
}

public static class Transformations
{
    public sealed class Mapping : CanPrintOutput
    {
        public Mapping(ITestOutputHelper output) : base(output)
        {
        }
        
        [Theory,
         InlineData("value"),
         InlineData(null)]
        public void Nullable(string? nullableValue)
        {
            var result = nullableValue is { } value ? value[..3] : null;
        
            WriteLine(result);
        }
    
        [Theory,
         OptionalInlineData("value"),
         OptionalInlineData(null!)]
        public void Optional(Option<string> maybeValue)
        {
            var result = maybeValue.Select(value => value[..3]);
        
            WriteLine(result);
        }
    }
    
    public sealed class Filtering : CanPrintOutput
    {
        public Filtering(ITestOutputHelper output) : base(output)
        {
        }
        
        [Theory,
         InlineData("value"),
         InlineData("other"),
         InlineData(null)]
        public void Nullable(string? nullableValue)
        {
            var result = nullableValue is { } value && value.StartsWith("val") ? value : null;
        
            WriteLine(result);
        }
        
        [Theory,
         OptionalInlineData("value"),
         OptionalInlineData("other"),
         OptionalInlineData(null!)]
        public void Optional(Option<string> maybeValue)
        {
            var result = maybeValue.Where(value => value.StartsWith("val"));
            
            WriteLine(result);
        }
    }
    
    public sealed class Binding : CanPrintOutput
    {
        public Binding(ITestOutputHelper output) : base(output)
        {
        }
        
        [Theory,
         BindingAutoData(hasClientId: true, "email", isAbleToSend: true),
         BindingAutoData(hasClientId: true, "email", isAbleToSend: false),
         BindingAutoData(hasClientId: false, "email", isAbleToSend: true),
         BindingAutoData(hasClientId: false, null, isAbleToSend: true)]
        public void Nullable(IEmailProviderNullable emailProvider, ISendNotificationsNullable notificationService, int? maybeClientId)
        {
            var nullableEmail = maybeClientId is { } clientId
                ? emailProvider.GetEmail(clientId)
                : null;
            
            Guid? nullableNotificationId = nullableEmail is { } email
                ? notificationService.SendNotification(email)
                : null;
            
            var message = nullableNotificationId is { } notificationId
                          && notificationId != Guid.Empty
                ? $"Notification {notificationId} is sent!"
                : null;

            #region Nested version

            // string? message = null;
            // if (maybeClientId is { } clientId)
            // {
            //     if (emailProvider.GetEmail(clientId) is { } email)
            //     {
            //         if (notificationService.SendNotification(email) is { } notificationId)
            //         {
            //             message = $"Notification {notificationId} is sent!";
            //         }
            //     }
            // }

            #endregion
        
            WriteLine(message);
        }
        
        [Theory,
         BindingAutoData(hasClientId: true, "email", isAbleToSend: true),
         BindingAutoData(hasClientId: true, "email", isAbleToSend: false),
         BindingAutoData(hasClientId: false, "email", isAbleToSend: true),
         BindingAutoData(hasClientId: false, null, isAbleToSend: true)]
        public void Optional(IEmailProviderOptional emailProvider, ISendNotificationsOptional notificationService, Option<int> maybeClientId)
        {
            var message = maybeClientId
                .Bind(emailProvider.GetEmail)
                .Bind(notificationService.SendNotification)
                .Where(notificationId => notificationId != Guid.Empty)
                .Select(notificationId => $"Notification {notificationId} is sent!");

            #region LINQ expression version

            // var message =
            //     from clientId in maybeClientId
            //     from email in emailProvider.GetEmail(clientId)
            //     from notificationId in notificationService.SendNotification(email)
            //     where notificationId != Guid.Empty
            //     select $"Notification {notificationId} is sent!";

            #endregion
            
            WriteLine(message);
        }

        #region Irrelevant
        
        public interface IEmailProviderNullable
        {
            string? GetEmail(int id);
        }
        
        public interface IEmailProviderOptional
        {
            Option<string> GetEmail(int id);
        }
        
        public interface ISendNotificationsNullable
        {
            Guid? SendNotification(string email);
        }
        
        public interface ISendNotificationsOptional
        {
            Option<Guid> SendNotification(string email);
        }
        
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        private sealed class BindingAutoDataAttribute : AutoDataAttribute
        {
            public BindingAutoDataAttribute(bool hasClientId, string? email, bool isAbleToSend)
                : base(() => CreateFixture(hasClientId, email, isAbleToSend))
            {
            }

            private static IFixture CreateFixture(bool hasClientId, string? email, bool isAbleToSend)
            {
                var fixture = new Fixture()
                    .Customize(new AutoNSubstituteCustomization());

                int? maybeClientId = hasClientId ? fixture.Create<int>() : null; 
                fixture.Inject(maybeClientId);
                fixture.Inject(Prelude.Optional(maybeClientId));

                fixture.Freeze<IEmailProviderNullable>()
                    .GetEmail(Arg.Any<int>())
                    .Returns(email);
                
                fixture.Freeze<IEmailProviderOptional>()
                    .GetEmail(Arg.Any<int>())
                    .Returns(Prelude.Optional(email));
                
                fixture.Freeze<ISendNotificationsNullable>()
                    .SendNotification(email!)
                    .Returns(isAbleToSend ? Guid.NewGuid() : Guid.Empty);

                fixture.Freeze<ISendNotificationsOptional>()
                    .SendNotification(email!)
                    .Returns(isAbleToSend ? Prelude.Some(Guid.NewGuid()) : Prelude.None);

                return fixture;
            }
        }

        #endregion
    }
    
    public sealed class CollapseOfTheContext : CanPrintOutput
    {
        public CollapseOfTheContext(ITestOutputHelper output) : base(output)
        {
        }
        
        [Theory,
         OptionalInlineData("42"),
         OptionalInlineData(null!)]
        public async Task IfNone(Option<string> maybeValue)
        {
            // somewhat similar to ?? operator
            var result = maybeValue.IfNone("Had nothing!");
            WriteLine(result);

            // Non-returning version
            maybeValue.IfNone(() => WriteLine("Nothing there"));
            
            // No collapsing method allows null
            Assert.Throws<ResultIsNullException>(() => maybeValue.IfNone(() => null!));
            
            // Holds for Nullable<T> as well, be aware
            Assert.Throws<ResultIsNullException>(() => Prelude.Optional<int?>(null).IfNone(() => null));

            // Most of the collapsing methods have Unsafe versions. Try to use it only on the borders of an application!
            var unsafeResult = maybeValue.IfNoneUnsafe((string?) null);
            WriteLine(unsafeResult ?? "NULL");

            // Most of them also have an async version (and even async unsafe versions!)
            var asyncResult = await maybeValue.IfNoneAsync(() => Task.FromResult("Async none!"));
            WriteLine(asyncResult);
            
            // IfSome has much way less overloads and is less useful in general
            maybeValue.IfSome(value => WriteLine($"Value is {value}"));
        }
        
        [Theory,
         OptionalInlineData("42"),
         OptionalInlineData(null!)]
        public void Match(Option<string> maybeValue)
        {
            string result = maybeValue.Match(
                value => $"Has value {value}",
                () => "Has no value!");
            WriteLine(result);

            Assert.Throws<ResultIsNullException>(() => maybeValue.Match<string>(_ => null!, () => null!));
            
            string? unsafeResult = maybeValue.MatchUnsafe(
                value => $"Has value {value}",
                () => null);
            WriteLine(unsafeResult ?? "NULL");
            
            // Switch (non-returning match)
            maybeValue.Match(
                value => WriteLine($"Switch to value {value}"),
                () => WriteLine("Switch to no value!"));
        }
    }
}
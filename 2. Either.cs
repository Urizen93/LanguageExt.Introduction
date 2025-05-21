namespace LanguageExt.Introduction;

public sealed class EitherExamples
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
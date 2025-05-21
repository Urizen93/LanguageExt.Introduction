namespace LanguageExt.Introduction.Examples.Exceptions;

public sealed class ConflictException : Exception
{
    public ConflictException(Exception ex) : base(null, ex)
    {
    }
}
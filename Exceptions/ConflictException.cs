namespace LanguageExt.Introduction.Exceptions;

public sealed class ConflictException : Exception
{
    public ConflictException(Exception ex) : base(null, ex)
    {
    }
}
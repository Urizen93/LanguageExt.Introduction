namespace LanguageExt.Introduction.Examples;

public interface IReturnResult
{
    OperationResult<T> GetResult<T>();
}
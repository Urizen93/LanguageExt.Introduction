using LanguageExt.Introduction.Examples;

namespace LanguageExt.Introduction.Services;

public interface IReturnResult
{
    OperationResult<T> GetResult<T>();
}
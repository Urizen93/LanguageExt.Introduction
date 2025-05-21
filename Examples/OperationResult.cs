using JetBrains.Annotations;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public class OperationResult<T>
{
    public OperationResult(bool isSuccess, string error, T value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public bool IsSuccess { get; set; }
      
    public string Error { get; set; }

    public T Value { get; set; }
}
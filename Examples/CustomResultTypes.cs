using JetBrains.Annotations;
using LanguageExt.Introduction.Services;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public sealed class CustomResultTypes
{
    private readonly IReturnResult _service;

    public CustomResultTypes(IReturnResult service) => _service = service;

    public string ValuesAreAccessibleWithoutAnyChecks()
    {
        OperationResult<string> result = _service.GetResult<string>();

        // Potential NRE or ignored error - we were supposed to check IsSuccess property
        // Everybody forgets to do so all the time
        // Ideally, a consumer must be forced to handle a bad result or discard it implicitly
        return result.Value.Trim();
    }

    public OperationResult<string> HardToTransform()
    {
        OperationResult<string> result = _service.GetResult<string>();

        // Tedious, error-prone transformations
        return new OperationResult<string>(
            result.IsSuccess,
            result.Error,
            result.IsSuccess
                ? result.Value[..3]
                : string.Empty);
    }

    public OperationResult<int> DoesNotCompose()
    {
        // Composition is annoying and requires a lot of branching
        // Imagine there were more than two results!
        OperationResult<int> first = _service.GetResult<int>();

        if (!first.IsSuccess) return first;
        
        OperationResult<int> second = _service.GetResult<int>();

        if (second.IsSuccess)
            return new OperationResult<int>(
                true,
                string.Empty,
                first.Value + second.Value);
            
        return new OperationResult<int>(
            false,
            string.Join(", ", [first.Error, second.Error]),
            default);
    }
}
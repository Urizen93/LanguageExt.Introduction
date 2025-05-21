using JetBrains.Annotations;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public sealed class CustomResultClasses
{
    private readonly IReturnResult _service;

    public CustomResultClasses(IReturnResult service) => _service = service;

    public string ValuesAreAccessibleWithoutAnyChecks()
    {
        var result = _service.GetResult<string>();

        // Potential NRE or ignored error - we were supposed to check IsSuccess property
        // Everybody forgets to do so all the time
        return result.Value.Trim();
    }

    public OperationResult<string> HardToTransform()
    {
        var result = _service.GetResult<string>();

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
        var first = _service.GetResult<int>();

        if (!first.IsSuccess) return first;
        
        var second = _service.GetResult<int>();

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
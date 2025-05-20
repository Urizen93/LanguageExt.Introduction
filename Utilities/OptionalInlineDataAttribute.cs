using System.Reflection;
using Xunit.Sdk;

namespace LanguageExt.Introduction.Utilities;

public sealed class OptionalInlineDataAttribute : DataAttribute
{
    private readonly object[] _data;
    
    public OptionalInlineDataAttribute(params string?[] data) => _data = data
        .Select(Prelude.Optional)
        .Cast<object>()
        .ToArray();
    
    public override IEnumerable<object[]> GetData(MethodInfo testMethod) => [_data];
}
using JetBrains.Annotations;

namespace LanguageExt.Introduction.Services;

[PublicAPI]
public interface IThrowingApi
{
    string? GetAddress(int customerId);
}
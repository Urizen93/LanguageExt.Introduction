using JetBrains.Annotations;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public interface IThrowingApi
{
    // What happens if there is no such customerId?
    // Does it return null? Or null is returned if a customer has no address? Who knows
    // Does it throw? If so, which exception? There is no point defining it in xml-doc - implementations can (and will) disregard that
    string? GetAddress(int customerId);
}
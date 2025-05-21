using JetBrains.Annotations;
using LanguageExt.Introduction.Examples.Exceptions;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public sealed class TryCatches
{
    private readonly IThrowingApi _service;

    public TryCatches(IThrowingApi service) => _service = service;

    public string? GetFormattedAddress()
    {
        try
        {
            var address = _service.GetAddress(1);
            // format it somehow
            return address;
        }
        catch (ApiSpecificException ex)
        {
            // We have to handle this exception here, otherwise the implementation details will leak to the levels above,
            // breaking the blackbox and making the api brittle, barely possible to change
            
            // What do we do if we don't know how to handle it on this level?
            // Imagine our app is supposed to return 409 on that. How do you communicate it to the controller?
            Console.WriteLine(ex);
            
            // So you wrap it into your own exception class, hoping it will be handled on the controller level - you have no way of enforcing it
            throw new ConflictException(ex);
        }
    }
}
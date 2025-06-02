using JetBrains.Annotations;
using LanguageExt.Introduction.Exceptions;
using LanguageExt.Introduction.Services;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public sealed class TryCatches
{
    private readonly IThrowingApi _service;

    public TryCatches(IThrowingApi service) => _service = service;

    // With exception-based approach, compiler is of no help for us :(
    public string? GetFormattedAddress(int customerId)
    {
        try
        {
            var address = _service.GetAddress(customerId);
            
            return FormatAddress(address);
        }
        catch (ApiSpecificException ex)
        {
            // We have to handle this exception here, otherwise the implementation details will leak to the levels above,
            // breaking the blackbox and making the api brittle, barely possible to change
            
            // What do we do if we don't know how to handle it on this level?
            // Imagine our app is supposed to return 409 on that. How do you communicate it to the controller?
            Console.WriteLine(ex);
            
            // So you wrap it into your own exception class, hoping it will be handled
            // on the controller level - you have no way of enforcing it
            throw new ConflictException(ex);
        }
    }

    private static string? FormatAddress(string? address) => address?.Trim();
}
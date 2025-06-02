using System.Net.Mail;
using LanguageExt.Introduction.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LanguageExt.Introduction.Examples;

public sealed class UserController : Controller
{
    private readonly IThrowingUserEmailProvider _service;

    public UserController(IThrowingUserEmailProvider service) => _service = service;

    public Results<NotFound, Ok<MailAddress>> GetUserEmail(int userId)
    {
        try
        {
            var email = _service.GetEmail(userId);

            return TypedResults.Ok(email);
        }
        // We aren't catching NoAccessException, which will lead to 500 error!
        catch (NotFoundException)
        {
            // If we changed the exception type or added a new exception,
            // we'd have to visit all the call sites to handle this (and the call sites of those call sites, and so on!)
            // And if we fail to do so, compiler won't be able to do anything about it, and runtime exceptions will follow
            return TypedResults.NotFound();
        }
    }
}
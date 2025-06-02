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
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
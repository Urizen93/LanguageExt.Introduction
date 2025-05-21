using System.Net.Mail;
using LanguageExt.Common;
using LanguageExt.Introduction.Models;

namespace LanguageExt.Introduction.Services;

public interface IReturnEither
{
    Either<NotFound, MailAddress> GetEmail(int clientId);
}
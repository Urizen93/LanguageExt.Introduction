using OneOf.Types;
using System.Net.Mail;

namespace LanguageExt.Introduction.Services;

public interface IReturnEither
{
    Either<NotFound, MailAddress> GetEmail(int clientId);
}
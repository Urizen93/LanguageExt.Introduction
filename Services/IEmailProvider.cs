using System.Net.Mail;
using LanguageExt.Common;

namespace LanguageExt.Introduction.Services;

public interface IEmailProvider
{
    Either<Error, MailAddress> GetEmail(int id);
}
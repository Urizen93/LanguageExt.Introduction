using System.Net.Mail;
using LanguageExt.Common;
using LanguageExt.Introduction.Models;

namespace LanguageExt.Introduction.Services;

public interface IEmailProvider
{
    Either<Error, MailAddress> GetEmail(int id);
}
using System.Net.Mail;
using LanguageExt.Common;

namespace LanguageExt.Introduction.Services;

public interface ISendNotifications
{
    Either<Error, Guid> SendNotification(MailAddress email);
}
using System.Net.Mail;

namespace LanguageExt.Introduction.Services;

public interface IUserEmailProvider
{
    // What happens if there is no such customerId?
    // Does it return null? Or null is returned if a customer has no address? Who knows
    MailAddress? GetMail(int userId);
}
using System.Net.Mail;
using JetBrains.Annotations;
using LanguageExt.Introduction.Exceptions;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public interface IThrowingUserEmailProvider
{
    // There is no point defining it in xml-doc - implementations can (and will) disregard that
    /// <exception cref="ArgumentException" />
    /// <exception cref="NotFoundException" />
    MailAddress GetEmail(int userId);
}
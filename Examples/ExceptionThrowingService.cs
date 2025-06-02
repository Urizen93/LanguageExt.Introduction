using System.Net.Mail;
using JetBrains.Annotations;
using LanguageExt.Introduction.Exceptions;
using LanguageExt.Introduction.Services;

namespace LanguageExt.Introduction.Examples;

[PublicAPI]
public sealed class ExceptionThrowingService : IThrowingUserEmailProvider
{
    #region Irrelevant

    private readonly ICheckAccessToUserData _accessChecker;
    private readonly IUserEmailProvider _emailProvider;

    public ExceptionThrowingService(
        ICheckAccessToUserData accessChecker,
        IUserEmailProvider emailProvider)
    {
        _accessChecker = accessChecker;
        _emailProvider = emailProvider;
    }

    #endregion

    public MailAddress GetEmail(int userId)
    {
        #region New requirements

        if (!_accessChecker.DoesCurrentUserHaveAccessTo(userId))
            throw new NoAccessException();

        #endregion

        return _emailProvider.GetMail(userId)
               ?? throw new NotFoundException();
    }
}
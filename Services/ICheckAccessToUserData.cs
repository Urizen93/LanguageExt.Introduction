namespace LanguageExt.Introduction.Services;

public interface ICheckAccessToUserData
{
    bool DoesCurrentUserHaveAccessTo(int userId);
}
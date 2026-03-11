using System.Security.Claims;

namespace Wrak.CleanArchitecture.Web.Identity;

public static class Claims
{
    public static class Administrator
    {
        public const string Type = "App";
        public const string Value = "App-Admin";
    }

    public static class User
    {
        public const string Type = "App";
        public const string Value = "App-User";
    }

    public static class DevUser
    {
        public const string Type = ClaimTypes.Name;
        public const string Value = "Dev-User";
    }

    public static class FirstName
    {
        public const string Type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
    }

    public static class LastName
    {
        public const string Type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
    }

    public static class FullName
    {
        public const string Type = "name";
    }

    public static class UserName
    {
        public const string Type = "preferred_username";
    }

    public static class Email
    {
        public const string Type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    }
}

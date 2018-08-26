using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NameThatTitle.Domain.Static
{
    public static class StaticData
    {
        public static class Forum
        {
            public static class Name
            {
                public const int MaxLength = 50;
            }
        }

        public static class Post
        {
            public static class Title
            {
                public const int MaxLength = 250;
            }
        }

        public static class Culture
        {
            public readonly static CultureInfo En = new CultureInfo("en");
            public readonly static CultureInfo Ru = new CultureInfo("ru");
        }

        public static class EmailTemplate
        {
            public const string ConfirmEmail = "ConfirmEmail";
            public const string EmailConfirmed = "EmailConfirmed";

            public const string PasswordResetValidateToken = "PasswordResetValidateToken";

            public const string ProfileHasChanged = "ProfileHasChanged"; // password, email, username

            public const string Notification = "Notification"; // new private message, new answer of your followed posts, new answer of your comment

            public const string Warning = "Warning"; // post or comment is inappropriate, it was hided
            public const string Banned = "Banned"; // user was banned
        }
    }
}

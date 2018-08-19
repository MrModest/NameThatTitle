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
    }
}

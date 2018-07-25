using System;
using System.Collections.Generic;
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
    }
}

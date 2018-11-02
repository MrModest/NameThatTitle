using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Core.Models.Users
{
    //ToDo: make it better
    public enum UserRolesEnum
    {
        Banned, // read-only
        User, // can create post and comments, reporting, up/down rating for posts and comments
        CantRate, // can't rate posts and comments
        Trusted, // can marked comments as solved
        Moderator, // can approve reported post and comment; can warn and ban users
        Admin // can delete/edit users, change roles
    }
}

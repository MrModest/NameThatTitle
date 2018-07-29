using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Domain.Models.Forum
{
    public enum SubscriptionType
    {
        AllNewMessages, // notify about all new comment in post (include offtopic)
        WithoutOfftopic, // notify if a new comment isn't marked as offtopic
        OnlyCorrectAnswer // notify only when a new comment is marked as correct answer
    }
}

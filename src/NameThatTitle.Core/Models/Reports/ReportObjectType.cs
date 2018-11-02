using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Core.Models.Reports
{
    public enum ReportObjectType
    {
        Post,
        Comment,
        User // if report not associated with concrete post/comment
    }
}

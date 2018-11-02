using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Core.Models.Reports
{
    public class Report : BaseEntity
    {
        public ReportObjectType ObjectType { get; set; }
        public int ObjectId { get; set; }
        public ReportReasonType ReasonType { get; set; }
        public string ReasonText { get; set; }
        public int ReportedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Considered { get; set; }
        public int? ModeratorId { get; set; } // who considered
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Core.Models.Forum
{
    public class Attachment : BaseEntity
    {
        public AttachmentType Type { get; set; }
        public string Link { get; set; }
    }
}

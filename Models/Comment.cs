﻿using ShadowBlog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }

        //We need to record the Id of the registered and logged in User who is creating these comments
        public string BlogUserId { get; set; }

        public string ModeratorId { get; set; }

        //Descriptive props
        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }
        public string CommentBody { get; set; }

        //The Moderator related properties
        public DateTime? Deleted { get; set; }

        public DateTime? Moderated { get; set; }
        public string ModeratedBody { get; set; }
        public ModType Moderationtype { get; set; }

        //Navigational props
        public virtual BlogPost BlogPost { get; set; }

        public virtual BlogUser BlogUser { get; set; }
        public virtual BlogUser Moderator { get; set; }
    }
}
﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public partial class Replies
    {
        public Replies()
        {
            InverseReplyByReplyNavigation = new HashSet<Replies>();
        }

        public int ReplyId { get; set; }
        public DateTime ReplyDate { get; set; }
        public string ReplyContent { get; set; }
        public int? ReplyByReply { get; set; }
        public int ProductId { get; set; }
        public int? Likes { get; set; }
        public bool IsRate { get; set; }
        public int? Rate { get; set; }
        public Guid? UserId { get; set; }
        [JsonIgnore]
        public Products Product { get; set; }
        [JsonIgnore]
        public Replies ReplyByReplyNavigation { get; set; }
        [JsonIgnore]
        public UserInfo User { get; set; }
        [JsonIgnore]
        public ICollection<Replies> InverseReplyByReplyNavigation { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fashion.Models
{
    public class ReplyPost_model
    {
        //回帖信息
        public int replyPostId;//回帖编号
        public int PostId;//帖子编号
        public string replyPostReplyerId;//回帖者编号
        public string replyPostContent;//回帖内容
        public int replyPostSupportCount;//点赞数
        public DateTime replyPostDate;//回帖日期
        public string replyPostHtmlUrl;//回帖帖子的静态页面地址
        public string firstPostPhotoUrl;//帖子里的一个张图片
        public int commentCount;//评论条数        
        public User_model Commenter;//User_model类，拥有用户的信息，回帖者
        public Post_model Post_model;//原贴类，拥有原贴的信息
        /// <summary>
        /// 构造函数
        /// </summary>
        public ReplyPost_model()
        {
            Commenter = new User_model();
            Post_model = new Post_model();
        }
    }
}
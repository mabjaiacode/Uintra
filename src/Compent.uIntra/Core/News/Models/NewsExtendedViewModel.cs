﻿using uCommunity.Comments;
using uCommunity.Likes;
using uCommunity.News;

namespace Compent.uIntra.Core.News.Models
{
    public class NewsExtendedViewModel : NewsViewModel
    {
        public ILikeable LikesInfo { get; set; }
        public ICommentable CommentsInfo { get; set; }
    }
}
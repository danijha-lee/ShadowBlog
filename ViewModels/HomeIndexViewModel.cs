using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShadowBlog.Models;

namespace ShadowBlog.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Blog> Blogs { get; set; } = new();

        public Blog MostRecentBlog { get; set; } = new();

        public BlogPost MostRecentPost { get; set; } = new();

        public BlogPost OldestPosts { get; set; } = new();

        public PostCardsViewModel HeaderCards { get; set; }
        public PostCardsViewModel NewestCards { get; set; }
        public PostCardsViewModel SpecificCategoryCards { get; set; }
        public PostCardsViewModel MostReadCards { get; set; }
    }
}
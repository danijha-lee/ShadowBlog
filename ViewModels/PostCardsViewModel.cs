using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShadowBlog.Models;

namespace ShadowBlog.ViewModels
{
    public class PostCardsViewModel
    {
        public Blog MainCard { get; set; }
        public List<Blog> SideCards { get; set; }
    }
}
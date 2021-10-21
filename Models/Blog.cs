using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        //Navigational property to reference all of my children
        public ICollection<BlogPost> BlogPosts = new HashSet<BlogPost>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShadowBlog.Data;

namespace ShadowBlog.Services
{
    public class BlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int CountPosts(int blogId)
        {
            return _context.BlogPosts.Where(b => b.BlogId == blogId).Count();
        }
    }
}
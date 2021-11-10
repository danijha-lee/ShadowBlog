using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShadowBlog.Data;
using ShadowBlog.Models;

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

        public async Task<List<Blog>> GetBlogDataAsync(int rows)
        {
            return await _context.Blogs.OrderBy(b => b.Created).Take(rows).ToListAsync();
        }

        public async Task<List<BlogPost>> GetBlogPostData(int rows)
        {
            var otherBlogPosts = await _context.BlogPosts.OrderByDescending(b => b.Created).ToListAsync();
            return otherBlogPosts;
        }
    }
}
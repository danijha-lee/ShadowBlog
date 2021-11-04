using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShadowBlog.Data;
using ShadowBlog.Enums;
using ShadowBlog.Models;

namespace ShadowBlog.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _dbcontext;

        public SearchService(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<BlogPost>> SearchAsync(string searchTerm)
        {
            List<BlogPost> posts = new();
            //If the user tries to search an enpty term
            if (!string.IsNullOrEmpty(searchTerm))

            {
                searchTerm = searchTerm.ToLower();
                //if the user enters something into the search input, perform the search
                posts = await _dbcontext.BlogPosts.Include(b => b.Blog)
                                             .Include(b => b.Comments)
                                             .ThenInclude(c => c.BlogUser)
                                             .Where(b => b.ReadyStatus == ReadyState.ProductionReady)
                                             .ToListAsync();
                posts = posts.Where(p => p.Title.ToLower().Contains(searchTerm) ||
                                         p.Abstract.ToLower().Contains(searchTerm) ||
                                         p.Content.ToLower().Contains(searchTerm) ||
                                         p.Blog.Name.ToLower().Contains(searchTerm) ||
                                         p.Comments.Any(c => c.CommentBody.ToLower().Contains(searchTerm) ||
                                         c.ModeratedBody.ToLower().Contains(searchTerm) ||
                                         c.BlogUser.FullName.ToLower().Contains(searchTerm))).ToList();
            }

            return posts;
        }
    }
}
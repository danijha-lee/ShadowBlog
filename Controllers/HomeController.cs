using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShadowBlog.Data;
using ShadowBlog.Models;
using ShadowBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
                                ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            PostCardsViewModel newestCards = new()
            {
                MainCard = await _context.Blogs.OrderByDescending(b => b.Created).FirstOrDefaultAsync(),
                SideCards = await _context.Blogs.OrderByDescending(b => b.Created).Skip(1).Take(4).ToListAsync(),
                MainPostCard = await _context.BlogPosts.OrderByDescending(b => b.Created).FirstOrDefaultAsync(),
                SidePostCards = await _context.BlogPosts.OrderByDescending(b => b.Created).Skip(1).Take(4).ToListAsync()
            };
            HomeIndexViewModel blogsvm = new()
            {
                Blogs = await _context.Blogs.ToListAsync(),
                MostRecentBlog = await _context.Blogs.OrderByDescending(b => b.Created).FirstOrDefaultAsync(),
                MostRecentPost = await _context.BlogPosts.OrderByDescending(b => b.Created).FirstOrDefaultAsync(),
                //OldestPosts = await _context.BlogPosts.OrderBy(p => p.Created).Take(4).ToListAsync(),
                NewestCards = newestCards,
            };

            return View(blogsvm);
        }

        public IActionResult ContactMe()
        {
            return View();
        }

        public IActionResult AboutMe()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
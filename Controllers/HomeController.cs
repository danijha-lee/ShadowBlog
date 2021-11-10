using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IEmailSender _emailService;
        private IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,
                                ApplicationDbContext context,
                                IEmailSender emailService,
                                IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            PostCardsViewModel newestCards = new()
            {
                MainCard = await _context.Blogs.OrderByDescending(b => b.Created).FirstOrDefaultAsync(),
                SideCards = await _context.Blogs.OrderByDescending(b => b.Created).ToListAsync(),
                MainPostCard = await _context.BlogPosts.OrderByDescending(b => b.Created).FirstOrDefaultAsync(),
                SidePostCards = await _context.BlogPosts.OrderByDescending(b => b.Created).ToListAsync()
            };
            HomeIndexViewModel blogsvm = new()
            {
                Blogs = await _context.Blogs.ToListAsync(),
                MostRecentBlog = await _context.Blogs.OrderByDescending(b => b.Created).FirstOrDefaultAsync(),
                MostRecentPost = await _context.BlogPosts.OrderByDescending(b => b.Created).FirstOrDefaultAsync(),
                OldestPosts = await _context.BlogPosts.OrderBy(p => p.Created).ToListAsync(),
                MainOldestPost = await _context.BlogPosts.OrderBy(p => p.Created).FirstOrDefaultAsync(),
                MainOldestBlog = await _context.Blogs.OrderBy(p => p.Created).FirstOrDefaultAsync(),
                OldestBlogs = await _context.Blogs.OrderBy(b => b.Created).ToListAsync(),

                NewestCards = newestCards,
            };

            return View(blogsvm);
        }
        public IActionResult ContactMe()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactMe(string name, string email, string phone, string message)
        {
            var subject = $"{name} has reached out to you from the ShadowBlog Application";

            var body = $"{message}.<br/><br/>{name} can be called at {phone} or emailed at {email} if follow up is required.";

            var myEmail = _configuration["SmtpSettings:Email"];
            await _emailService.SendEmailAsync(myEmail, subject, body);
            return RedirectToAction(nameof(Index));
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
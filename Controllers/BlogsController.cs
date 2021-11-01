using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShadowBlog.Data;
using ShadowBlog.Models;
using ShadowBlog.Services.Interfaces;
using ShadowBlog.ViewModels;

namespace ShadowBlog.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;

        public BlogsController(ApplicationDbContext context,
                            IImageService imageService,
                            UserManager<BlogUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
        }

        // GET: Blogs
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
            //return View(await _context.Blogs.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                if (blog.Image is null)
                {
                    blog.ImageData = await _imageService.EncodeImageAsync("defualtBlog.jpg");
                    blog.ContentType = "jpg";
                }
                else
                {
                    if (!_imageService.ValidImage(blog.Image))
                    {
                        //We need to add a custom Model Error and inform the user
                        ModelState.AddModelError("Image", "Please choose a valid image");
                        return View(blog);
                    }
                    else
                    {
                        blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                        blog.ContentType = _imageService.ContentType(blog.Image);
                    }
                }

                blog.Created = DateTime.Now;
                blog.UserId = _userManager.GetUserId(User);

                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Created,Image,ImageData,ContentType,UserId")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (blog.Image is not null)
                    {
                        if (!_imageService.ValidImage(blog.Image))
                        {
                            //We need to add a custom Model Error and inform the user
                            ModelState.AddModelError("Image", "Please choose a valid image");
                            return View(blog);
                        }
                        else
                        {
                            blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                            blog.ContentType = _imageService.ContentType(blog.Image);
                        }
                    }

                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShadowBlog.Data;
using ShadowBlog.Models;
using ShadowBlog.Services.Interfaces;
using ShadowBlog.Enums;
using Microsoft.AspNetCore.Authorization;
using ShadowBlog.Services;
using X.PagedList;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ShadowBlog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly ISlugService _slugService;
        private readonly SearchService _searchService;
        private readonly UserManager<BlogUser> _userManager;

        public BlogPostsController(ApplicationDbContext context,
                                    IImageService imageService,
                                    ISlugService slugService,
                                    SearchService searchService,
                                    UserManager<BlogUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            _slugService = slugService;
            _searchService = searchService;
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SearchPosts(int? page, string searchTerm)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;
            ViewData["SearchTerm"] = searchTerm;

            var blogPosts = await _searchService.SearchAsync(searchTerm);
            if (blogPosts.Count == 0)
            {
                ViewData["Message"] = "No Posts Found Matching your search term. Please try searching something else";
            }

            return View("SearchPosts", await blogPosts.ToPagedListAsync(pageNumber, pageSize));
        }

        public async Task<IActionResult> ChildIndex(int blogId, int? page)
        {
            //I don't want to get all of the BlogPosts....
            //I want to get all of the BlogPosts where the BlogId = blogId
            //Also...I only want to grab production ready BlogPosts
            var pageNumber = page ?? 1;
            var pageSize = 5;

            var applicationDbContext = _context.BlogPosts
                .Include(b => b.Blog)
                .Where(b => b.ReadyStatus == ReadyState.ProductionReady && b.BlogId == blogId)
                .OrderByDescending(b => b.Created);

            var blogPosts = await applicationDbContext.ToPagedListAsync(pageNumber, pageSize);

            return View(blogPosts);
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;

            var applicationDbContext = _context.BlogPosts
                .Include(b => b.Blog)
                .Where(b => b.ReadyStatus == ReadyState.ProductionReady)
                .OrderByDescending(b => b.Created);

            var blogPosts = await applicationDbContext.ToPagedListAsync(pageNumber, pageSize);

            return View(blogPosts);
        }

        public async Task<IActionResult> Preview()
        {
            var blogPosts = _context.BlogPosts
               .Include(b => b.Blog)
               .Where(b => b.ReadyStatus == ReadyState.InPreview)
               .OrderByDescending(b => b.Created);

            return View("Index", await blogPosts.ToListAsync());
        }

        // GET: BlogPosts/Details/5

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .Include(b => b.Tags)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.Moderator)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        public async Task<IActionResult> TagIndex(string tag, int? page)
        {
            //Start with my pageing data
            var pageNumber = page ?? 1;
            var pageSize = 6;

            var allBlogPostIds = _context.Tags.Where(t => t.Text.ToLower() == tag.ToLower())
                                              .Select(t => t.BlogPostId);

            var blogPosts = await _context.BlogPosts
                                        .Where(b => allBlogPostIds.Contains(b.Id))
                                        .OrderByDescending(b => b.Created)
                                        .ToPagedListAsync(pageNumber, pageSize);

            ViewData["Tag"] = tag;
            return View(blogPosts);
        }

        //GET: BlogPosts/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(int? blogId)
        {
            if (blogId is not null)
            {
                BlogPost newBlogPost = new()
                {
                    BlogId = (int)blogId
                };

                return View(newBlogPost);
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] BlogPost blogPost, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                //Let's create and check the slug for uniqueness
                var slug = _slugService.UrlFriendly(blogPost.Title);
                if (!_slugService.IsUnique(slug))
                {
                    //Create a custom Model Error and complain to the user
                    ModelState.AddModelError("Title", "Error: Title has already been used.");
                    return View(blogPost);
                }
                else
                {
                    blogPost.Slug = slug;
                }

                //Either record the incoming image or use the default image
                if (blogPost.Image is not null)
                {
                    if (!_imageService.ValidImage(blogPost.Image))
                    {
                        if (!_imageService.ValidType(blogPost.Image))
                        {
                            ModelState.AddModelError("Image", "Please choose a valid image type.");
                            return View(blogPost);
                        }
                        else
                        {
                            ModelState.AddModelError("Image", "Please choose a valid image size.");
                            return View(blogPost);
                        }
                    }
                    else
                    {
                        blogPost.ImageData = await _imageService.EncodeImageAsync(blogPost.Image);
                        blogPost.ImageType = _imageService.ContentType(blogPost.Image);
                    }
                }
                else
                {
                    blogPost.ImageData = await _imageService.EncodeImageAsync("blogPostDefaultImage.png");
                    blogPost.ImageType = "png";
                }

                blogPost.Created = DateTime.Now;

                //TODO: Add code in here to generate the Slug from the Title

                _context.Add(blogPost);
                await _context.SaveChangesAsync();

                foreach (var tag in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        BlogPostId = blogPost.Id,
                        Text = tag
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", blogPost.BlogId);
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["TagValues"] = string.Join(",", blogPost.Tags.Select(t => t.Text));
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", blogPost.BlogId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,Created,ReadyStatus,ImageData,ImageType,Slug,Image")] BlogPost blogPost, List<string> tagValues)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //In the Edit we have to make sure that the Title actually changed before checking slug uniqueness
                    var slug = _slugService.UrlFriendly(blogPost.Title);
                    if (slug != blogPost.Slug)
                    {
                        if (!_slugService.IsUnique(slug))
                        {
                            //Create a custom Model Error and complain to the user
                            ModelState.AddModelError("Title", "Error: Title has already been used.");
                            return View(blogPost);
                        }
                        else
                        {
                            blogPost.Slug = slug;
                        }
                    }
                    if (blogPost.Image is not null)
                    {
                        if (!_imageService.ValidImage(blogPost.Image))
                        {
                            //We need to add a custom Model Error and inform the user
                            ModelState.AddModelError("Image", "Please choose a valid image");
                            return View(blogPost);
                        }
                        else
                        {
                            blogPost.ImageData = await _imageService.EncodeImageAsync(blogPost.Image);
                            blogPost.ImageType = _imageService.ContentType(blogPost.Image);
                        }
                    }

                    blogPost.BlogUser = await _userManager.GetUserAsync(User);
                    blogPost.Updated = DateTime.Now;
                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();

                    BlogPost tempPost = await _context.BlogPosts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == blogPost.Id);
                    _context.Tags.RemoveRange(tempPost.Tags);

                    foreach (var tag in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            BlogPostId = blogPost.Id,
                            Text = tag
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "BlogPosts", new { slug = blogPost.Slug });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", blogPost.BlogId);
            // return View(blogPost);
            return RedirectToAction("Details", "BlogPosts", new { slug = blogPost.Slug });
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
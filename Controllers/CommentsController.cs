using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShadowBlog.Data;
using ShadowBlog.Models;
using ShadowBlog.Enums;

namespace ShadowBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        [Authorize(Roles = "Moderator")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comment.Include(c => c.BlogPost).Include(c => c.BlogUser).Include(c => c.Moderator);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.BlogPost)
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogPostId,CommentBody")] Comment comment, string slug)
        {
            if (ModelState.IsValid)
            {
                comment.Created = DateTime.Now;

                //I need an injected instance of UserManager...
                comment.BlogUserId = _userManager.GetUserId(User);

                _context.Add(comment);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                var blogPost = _context.BlogPosts.Find(comment.BlogPostId);
                //Return the user back to the Details page
                return RedirectToAction("Details", "BlogPosts", new { slug = blogPost.Slug }, "fragComment");
            }

            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int commentId, string body, string slug)
        {
            if (commentId == 0)
                return NotFound();

            try
            {
                var comment = await _context.Comment.FindAsync(commentId);
                comment.CommentBody = body;
                comment.Updated = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "BlogPosts", new { slug }, "fragComment");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(commentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Moderate(int commentId, int moderationType, string moderatedBody, string slug)
        {
            var comment = await _context.Comment.FindAsync(commentId);
            comment.Moderated = DateTime.Now;
            comment.ModeratorId = _userManager.GetUserId(User);
            comment.Moderationtype = (ModType)moderationType;
            comment.ModeratedBody = moderatedBody;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "BlogPosts", new { slug }, "fragComment");
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.BlogPost)
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string slug)
        {
            var comment = await _context.Comment.FindAsync(id);
            comment.Deleted = DateTime.Now;
            _context.Update(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "BlogPosts", new { slug });
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
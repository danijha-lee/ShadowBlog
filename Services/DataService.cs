using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShadowBlog.Data;
using ShadowBlog.Models;
using ShadowBlog.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowBlog.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISlugService _slugService;

        public DataService(ApplicationDbContext dbContext,
            IImageService imageService,
            UserManager<BlogUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ISlugService slugService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
            _userManager = userManager;
            _roleManager = roleManager;
            _slugService = slugService;
        }

        //I am free to use _dbContext anywhere inside the class...
        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();

            //Step 1: Create 2 Roles
            await SeedRolesAsync();

            //Step 2: Create 2 Users and assign the roles
            await SeedUsersAsync();

            ////Step 3: Seed Blogs -- For paging purposes...
            //await SeedBlogsAsync();

            ////Step 4: Seed Posts for purposes of working with paging and forwarding the search term
            //await SeedBlogPostsAsync();
        }

        private async Task SeedRolesAsync()
        {
            //Ask if there are any Roles in the AspNetRoles table
            if (_dbContext.Roles.Any())
            {
                return;
            }

            IdentityRole adminRole = new("Administrator");
            await _roleManager.CreateAsync(adminRole);

            IdentityRole moderatorRole = new("Moderator");
            await _roleManager.CreateAsync(moderatorRole);
        }

        private async Task SeedUsersAsync()
        {
            //Ask if there are any Users at all already in the AspNetUsers table
            if (_dbContext.Users.Count() > 2)
            {
                return;
            }

            BlogUser admin = new()
            {
                Email = "danijha.monaelee@gmail.com",
                UserName = "danijha.monaelee@gmail.com",
                FirstName = "Monae",
                LastName = "Lee",
                PhoneNumber = "555-1212",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync("IMG-5700.jpg"),
                ImageType = "jpg"
            };

            BlogUser mod2 = new()
            {
                Email = "theemonaelee@gmail.com",
                UserName = "theemonaelee@gmail.com",
                FirstName = "Monae",
                LastName = "Lee",
                PhoneNumber = "555-1212",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync("IMG-5700.jpg"),
                ImageType = "jpg"
            };

            BlogUser mod = new()
            {
                Email = "JasonTwichell@Mailinator.com",
                UserName = "JasonTwichell@Mailinator.com",
                FirstName = "Jason",
                LastName = "Twichell",
                PhoneNumber = "555-1212",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync("defualtAccountImage.jpg"),
                ImageType = "jpg"
            };

            await _userManager.CreateAsync(admin, "Abc&123!");
            await _userManager.CreateAsync(mod2, "Abc&123!");
            await _userManager.CreateAsync(mod, "Abc&123!");
            await _userManager.AddToRoleAsync(admin, "Administrator");
            await _userManager.AddToRoleAsync(mod, "Moderator");
            await _userManager.AddToRoleAsync(mod2, "Moderator");

            //TODO: Now Seed a User who will occupy the Moderator role
        }

        //private async Task SeedBlogPostsAsync()
        //{
        //    if (_dbContext.BlogPosts.Any())
        //        return;

        //    var blogId = (await _dbContext.Blogs.AsNoTracking().OrderBy(b => b.Created).FirstOrDefaultAsync()).Id;
        //    for (var loop = 1; loop <= 20; loop++)
        //    {
        //        var title = $"Post number {loop}";
        //        _dbContext.Add(new BlogPost()
        //        {
        //            BlogId = blogId,
        //            Title = title,
        //            ReadyStatus = Enums.ReadyState.ProductionReady,
        //            Slug = _slugService.UrlFriendly(title),
        //            Abstract = $"Abstract for Posts number {loop}",
        //            Content = $"Content of Post number {loop}",
        //            Created = DateTime.Now.AddDays(loop),
        //            ImageData = await _imageService.EncodeImageAsync("defaultBlogPost.jpg"),
        //            ImageType = "jpg"
        //        });
        //    }
        //    await _dbContext.SaveChangesAsync();
        //}

        //private async Task SeedBlogsAsync()
        //{
        //    if (_dbContext.Blogs.Any())
        //        return;

        //    for (var loop = 1; loop <= 20; loop++)
        //    {
        //        _dbContext.Add(new Blog()
        //        {
        //            Name = $"Blog For Application {loop}",
        //            Description = $"Everything I learned while building Application {loop}",
        //            Created = DateTime.Now.AddDays(loop),
        //            ImageData = await _imageService.EncodeImageAsync("defaultBlog.jpg"),
        //            ContentType = "jpg"
        //        });
        //    }
        //    await _dbContext.SaveChangesAsync();
        //}
    }
}
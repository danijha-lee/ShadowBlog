using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShadowBlog.Data;
using ShadowBlog.Models;

namespace ShadowBlog.Controllers.APIContollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        //provide EndPoint to User
        //LocalHost:5001/API/PostsService/GetTopXPosts/1

        //HTTP: GET
        /// <summary>
        /// Allow A Consumer to request the lastest x number of blogPosts
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet("/GetTopXPosts/{num}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTopXPosts(int num)
        {
            return await _context.BlogPosts.OrderByDescending(p => p.Created).Take(num).ToListAsync();
        }
    }
}
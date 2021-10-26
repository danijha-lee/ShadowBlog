using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} property must be at least {2} character and at most {1} characters long", MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} property must be at least {2} character and at most {1} characters long", MinimumLength = 5)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }

        public string UserId { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        //Add a navigational property to reference all of my children
        public ICollection<BlogPost> BlogPosts = new HashSet<BlogPost>();

        public virtual BlogUser User { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowBlog.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public int BlogPostId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} property must be at least {2} character and at most {1} characters long", MinimumLength = 2)]
        public string Text { get; set; }

        //Navigational Properties
        public virtual BlogPost BlogPost { get; set; }
    }
}
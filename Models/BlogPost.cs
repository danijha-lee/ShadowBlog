using Microsoft.AspNetCore.Http;
using ShadowBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowBlog.Models
{
    public class BlogPost
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        /// <remarks>This Is the Id that is stored in the database</remarks>

        public int Id { get; set; }

        /// <summary>
        /// Foreign Key
        /// </summary>
        //The Foreign Key (FK)
        public int BlogId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 4)]
        public string Title { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        //Add a property of type bool named ProductionReady
        [Required]
        [Display(Name = "Ready Status")]
        public ReadyState ReadyStatus { get; set; }

        //This will basically be the Title run through a formatter
        // Role Based Security - role-based-security
        public string Slug { get; set; }

        //This represents the byte data not the physical file
        public byte[] ImageData { get; set; }

        public string ImageType { get; set; }

        //This property represents a physical file chosen by the user
        [NotMapped]
        public IFormFile Image { get; set; }

        //Navigational properties
        public virtual Blog Blog { get; set; }

        public virtual BlogUser BlogUser { get; set; }

        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ShadowBlog.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at most {1} characteers long.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at most {1} characteers long.")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at most {1} characteers long.")]
        public string DisplayName { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
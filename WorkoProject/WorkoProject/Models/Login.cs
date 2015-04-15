using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkoProject.Models
{
    public class Login
    {
        [Display(Name = "סיסמא:")]
        [Required(ErrorMessage = "אנא הזן סיסמא.")]
        public string Password { get; set; }

        [Display(Name = "תעודת זהות:")]
        [Required(ErrorMessage = "אנא הזן תעודת זהות.")]
        public string IdNumber { get; set; }

        [Display(Name = "זכור אותי")]
        public bool RememberME { get; set; }
    }
}
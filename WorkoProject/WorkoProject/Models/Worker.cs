using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WorkoProject.Models
{
    public class Worker
    {
        [Display(Name = "שם פרטי:")]
        [Required(ErrorMessage = "אנא הכנס שם פרטי.")]
        public string FirstName { get; set; }

        [Display(Name = "שם משפחה:")]
        [Required(ErrorMessage = "אנא הכנס שם משפחה.")]
        public string LastName { get; set; }

        [Display(Name = "מייל:")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "אנא הכנס כתובת מייל תקינה.")]
        public string Email { get; set; }

        [Display(Name = "טלפון:")]
        public string Phone { get; set; }

        [Display(Name = "סיסמא:")]
        [Required(ErrorMessage = "אנא הכנס סיסמא.")]
        public string Password { get; set; }

        [Display(Name = "תעודת זהות:")]
        [Required(ErrorMessage = "אנא הכנס תעודת זהות.")]
        [IdNumberValidate]
        public string IdNumber { get; set; }

        public bool IsAdmin { get; set; }

        [Display(Name = "תמונת פרופיל:")]
        public string Picture { get; set; }
    }
}

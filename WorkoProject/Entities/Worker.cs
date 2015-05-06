using Entities.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [DataContract]
    public class Worker
    {
        [DataMember]
        [Display(Name = "שם פרטי:")]
        [Required(ErrorMessage = "אנא הכנס שם פרטי.")]
        public string FirstName { get; set; }

        [Display(Name = "שם משפחה:")]
        [Required(ErrorMessage = "אנא הכנס שם משפחה.")]
        [DataMember]
        public string LastName { get; set; }

        [Display(Name = "מייל:")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "אנא הכנס כתובת מייל תקינה.")]
        [DataMember]
        public string Email { get; set; }

        [Display(Name = "טלפון:")]
        [DataMember]
        public string Phone { get; set; }

        [Display(Name = "סיסמא:")]
        [Required(ErrorMessage = "אנא הכנס סיסמא.")]
        [DataMember]
        public string Password { get; set; }

        [Display(Name = "תעודת זהות:")]
        [Required(ErrorMessage = "אנא הכנס תעודת זהות.")]
        [IdNumberValidate]
        [DataMember]
        public string IdNumber { get; set; }

        [DataMember]
        public bool IsAdmin { get; set; }

        [Display(Name = "תמונת פרופיל:")]
        [DataMember]
        public string Picture { get; set; }

        [Display(Name = "סוג עובד:")]
        [Required]
        [Range(1, 3, ErrorMessage = "אנא בחר סוג מהרשימה.")]
        public WorkerType Type { get; set; }

        public int ShiftCounter { get; set; }

        public int NightsCounter { get; set; }

        
    }
}

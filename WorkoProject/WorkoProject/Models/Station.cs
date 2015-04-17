using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoProject.Models
{
    public class Station
    {
        public int Id { get; set; }

        [Display(Name = "שם העמדה")]
        [Required(ErrorMessage = "אנא הזן שם עמדה.")]
        public string Name { get; set; }
        
        [Display(Name = "תיאור")]
        public string Description { get; set; }

        public StationStatus Status { get; set; }
        
        public bool[][] Constrains { get; set; }
    }
}

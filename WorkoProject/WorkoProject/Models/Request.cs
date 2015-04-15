using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoProject.Utils;

namespace WorkoProject.Models
{
    public class Request
    {
        public Request()
        {
            IsNewRequest = true;
            WorkerId = SessionManager.CurrentWorker.IdNumber;
        }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public bool IsNewRequest { get; set; }

        public string WorkerId { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Station
    {
        public Station()
        {
            Workers = new List<Worker>();
        }

        public Station(Station station)
        {
            this.Id = station.Id;
            this.Name = station.Name;
            this.Status = station.Status;
            this.Description = station.Description;
            this.Constrains = station.Constrains;
            this.NumberOfWorkers = station.NumberOfWorkers;
            this.Priority = station.Priority;
            this.Workers = new List<Worker>();
        }

        public int Id { get; set; }

        [Display(Name = "שם העמדה")]
        [Required(ErrorMessage = "אנא הזן שם עמדה.")]
        public string Name { get; set; }

        [Display(Name = "תיאור")]
        public string Description { get; set; }

        public StationStatus Status { get; set; }

        public bool[][] Constrains { get; set; }

        public int Priority { get; set; }

        public int NumberOfWorkers { get; set; }

        public List<Worker> Workers { get; set; }
    }
}

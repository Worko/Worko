using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoProject.Models
{
    public class WorkSchedule
    {
        public WorkSchedule(int wsid)
        {
            WSID = wsid;
        }

        public int WSID { get; set; }

        public WorkScheduleTemplate Template { get; set; }

        #region Algorithm

        private void GenerateWorkSchedule()
        {
            DBService.DB clnt = new DBService.DB();

            var workers = clnt.GetWorkers();
            var stations = clnt.GetStations(Entities.StationStatus.None);
            //var workersConstrains = null;///TODO: get all workers constrains
                                        

        }

        private double CalculateWorkerGrade()
        {
            double grade = 0;

            return grade;
        }

        #endregion Algorithm

    }

}

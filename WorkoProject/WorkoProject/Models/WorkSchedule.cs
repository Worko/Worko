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

            var wsid = clnt.GetWSID();

            var workers = clnt.GetWorkers();
            var stations = clnt.GetStations(Entities.StationStatus.None);
            var workersConstrains = clnt.GetAllWorkersConstrains(wsid);
            //var stationsConstrains = clnt.GetStationConstrains(wsid);
            
            // Sort stations by priority
            var sortedStationsConstrains = clnt.GetSortedStationConstrains(wsid);

        }

        private double CalculateWorkerGrade()
        {
            double grade = 0;

            return grade;
        }

        #endregion Algorithm

    }

}

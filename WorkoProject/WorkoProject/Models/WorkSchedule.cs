using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoProject.Models
{
    public class WorkSchedule
    {
        private DBService.DB clnt = new DBService.DB();

        public WorkSchedule(int wsid)
        {
            WSID = wsid;
        }

        public int WSID { get; set; }

        public WorkScheduleTemplate Template { get; set; }

        #region Algorithm

        private void GenerateWorkSchedule()
        {
            

            WSID = clnt.GetWSID();

            var workers = clnt.GetWorkers();
            var stations = clnt.GetStations(Entities.StationStatus.None);
            var workersConstrains = clnt.GetAllWorkersConstrains(WSID);
            
            //var stationsConstrains = clnt.GetStationConstrains(wsid);
            
            // Sort stations by shift then priority 
            var sortedStationsConstrains = clnt.GetSortedStationConstrains(WSID);

            for (int i = 0; i < sortedStationsConstrains.Count; i++)
            {
                for (int j = 0; j < sortedStationsConstrains[i].NumberOfWorkers; j++)
                {
                    
                }
            }

        }

        private List<Tuple<string, double>> CalculateWorkersGrade(int stationId, int day, int shift)
        {
            var workersByStation = clnt.GetWorkersByStationID(stationId);
            List<Tuple<string, double>> grades = new List<Tuple<string,double>>();
            

            foreach (var w in workersByStation)
            {
                double grade = 0;
                int shiftIndex = Shift.GetShiftIndex((DayOfWeek)day, (Entities.PartOfDay)shift);

                ///TODO: check type
                grade += 1;
                ///

                var workerConstrains = clnt.GetWorkerConstrains(w, WSID);
                if (workerConstrains[shiftIndex])
                {
                    continue;
                }

                grade += 1;

                


            }
            return grades;
        }

        private bool IsWorkerWorkBeforeOrAfter(int day, int shift, int stationId, string workerID)
        {
            DayOfWeek d = (DayOfWeek)day;
            Entities.PartOfDay s = (Entities.PartOfDay)shift;

            int currentShiftIndex = Shift.GetShiftIndex(d, s);
            int prevShiftIndex = Shift.GetPreviousShiftIndex(d, s) ;
            int nextShiftIndex = Shift.GetNextShiftIndex(d, s);

            Worker worker = null;
            try
            {
                // check if worker already work in current shift
                worker = this.Template.Shifts[currentShiftIndex].Stations.Find(x => x.Id == stationId).Workers.Find(x => x.IdNumber == workerID);
            }
            catch { }

            return worker != null;
        }

        #endregion Algorithm

    }

}

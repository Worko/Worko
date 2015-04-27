using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoProject.Utils
{
    public class WorkoAlgorithm
    {
        private static DBService.DB clnt = new DBService.DB();
        private static WorkSchedule workSchedulel;

        #region Algorithm

        public static void GenerateWorkSchedule()
        {
            int wsid = clnt.GetWSID();
            workSchedulel = new WorkSchedule(wsid);

            var workers = clnt.GetWorkers();
            var stations = clnt.GetStations(Entities.StationStatus.None);
            var workersConstrains = clnt.GetAllWorkersConstrains(workSchedulel.WSID);

            //var stationsConstrains = clnt.GetStationConstrains(wsid);

            // Sort stations by shift then priority 
            var sortedStationsConstrains = clnt.GetSortedStationConstrains(workSchedulel.WSID);

            for (int i = 0; i < sortedStationsConstrains.Count; i++)
            {
                for (int j = 0; j < sortedStationsConstrains[i].NumberOfWorkers; j++)
                {

                }
            }

        }

        private static List<Tuple<string, double>> CalculateWorkersGrade(int stationId, int day, int shift)
        {
            var workersByStation = clnt.GetWorkersByStationID(stationId);
            List<Tuple<string, double>> grades = new List<Tuple<string, double>>();


            foreach (var w in workersByStation)
            {
                double grade = 0;
                int shiftIndex = Shift.GetShiftIndex((DayOfWeek)day, (Entities.PartOfDay)shift);

                ///TODO: check type
                grade += 1;
                ///

                var workerConstrains = clnt.GetWorkerConstrains(w, workSchedulel.WSID);
                if (workerConstrains[shiftIndex] && !IsWorkerWorkBeforeOrAfter(day, shift, stationId, w))
                {
                    continue;
                }

                grade += 1;




            }
            return grades;
        }

        private static bool IsWorkerWorkBeforeOrAfter(int day, int shift, int stationId, string workerID)
        {
            DayOfWeek d = (DayOfWeek)day;
            PartOfDay s = (PartOfDay)shift;

            int currentShiftIndex = Shift.GetShiftIndex(d, s);
            int prevShiftIndex = Shift.GetPreviousShiftIndex(d, s);
            int nextShiftIndex = Shift.GetNextShiftIndex(d, s);

            foreach (var station in workSchedulel.Template.Shifts[currentShiftIndex].Stations)
            {
                if (station.Workers.Find(x => x.IdNumber == workerID) != null)
                {
                    return true;
                }
            }

            ///TODO: if sunday check saturday of prev week
            foreach (var station in workSchedulel.Template.Shifts[prevShiftIndex].Stations)
            {
                if (station.Workers.Find(x => x.IdNumber == workerID) != null)
                {
                    return true;
                }
            }

            foreach (var station in workSchedulel.Template.Shifts[nextShiftIndex].Stations)
            {
                if (station.Workers.Find(x => x.IdNumber == workerID) != null)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Algorithm
    }
}

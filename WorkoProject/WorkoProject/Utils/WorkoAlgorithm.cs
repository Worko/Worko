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
        private static int WSID = clnt.GetWSID();
        private static WorkSchedule workSchedule = new WorkSchedule(WSID);
        private static List<Worker> workers = clnt.GetWorkers();
        private static List<Station> stations = clnt.GetStations(Entities.StationStatus.None);
        private static List<WorkerConstrains> workersConstrains = clnt.GetAllWorkersConstrains(workSchedule.WSID);
        private static List<string> lastSaturdayNightWorkers = clnt.GetLastSaturdayNightWorkers(WSID);
        private static List<Tuple<int, string>> WorkersStations = clnt.GetWorkersStations();
        //var stationsConstrains = clnt.GetStationConstrains(wsid);

        // Sort stations by shift then priority 
        private static List<SortedScheduleConstrains> sortedStationsConstrains = clnt.GetSortedStationConstrains(workSchedule.WSID);

        #region Algorithm

        private static void Init()
        {

        }

        public static void GenerateWorkSchedule()
        {
            for (int i = 0; i < sortedStationsConstrains.Count; i++)
            {
                int sid = sortedStationsConstrains[0].StationId;
                int day = sortedStationsConstrains[0].Day;
                int shift = sortedStationsConstrains[0].ShiftTime;
                int maxWorkers = sortedStationsConstrains[i].NumberOfWorkers;
                var fitsWorkers = CalculateWorkersGrade(sid, day, shift, maxWorkers);
                AddWorkersToSchedule(sid, day, shift, fitsWorkers);
            }

        }

        private static void AddWorkersToSchedule(int stationId, int day, int shift, List<string> fitsWorkers)
        {
            List<Worker> fitWorkersList = new List<Worker>();
            foreach (string w in fitsWorkers)
            {
                fitWorkersList.Add(workers.Find(x => x.IdNumber == w));
            }

            var station = workSchedule.Template
                                      .Shifts[Shift.GetShiftIndex((DayOfWeek)day, (PartOfDay)shift)]
                                      .Stations.Find(x => x.Id == stationId);

            if (station != null)
            {
                 workSchedule.Template
                             .Shifts[Shift.GetShiftIndex((DayOfWeek)day, (PartOfDay)shift)]
                             .Stations.Find(x => x.Id == stationId).Workers.AddRange(fitWorkersList);
            }
            else
            {
                station = stations.Find(x => x.Id == stationId);
                station.Workers.AddRange(fitWorkersList);

                workSchedule.Template
                            .Shifts[Shift.GetShiftIndex((DayOfWeek)day, (PartOfDay)shift)]
                            .Stations.Add(station);
            }
            
        }

        private static List<string> CalculateWorkersGrade(int stationId, int day, int shift, int maxWorkers)
        {
            List<string> workersByStation = new List<string>();
            var ws = WorkersStations.FindAll(x => x.Item1 == stationId);
            foreach (var w in ws)
            {
                workersByStation.Add(w.Item2);
            }

            List<Tuple<string, double>> grades = new List<Tuple<string, double>>();

            foreach (var w in workersByStation)
            {
                
                double grade = 0;
                int shiftIndex = Shift.GetShiftIndex((DayOfWeek)day, (Entities.PartOfDay)shift);

                ///TODO: check type
                grade += 1;
                ///
                WorkerConstrains workerConstrains = workersConstrains.Find(x => x.WorkerID.TrimStart('0') == w);
                if (workerConstrains.Constrains[day][shift] == true || IsWorkerWorkBeforeOrAfter(day, shift, stationId, w))
                {
                    continue;
                }

                grade += 1;

            }

            grades = grades.OrderByDescending(x => x.Item2).ToList();

            List<string> intendedWorkers = new List<string>();

            for (int j = 0; j < maxWorkers && j < grades.Count; j++)
            {
                var temp = grades.FindAll(x => x.Item2 == grades[0].Item2);
                int index = new Random().Next() % temp.Count;
                intendedWorkers.Add(temp[index].Item1);
                grades.Remove(temp[index]);
            }

            return intendedWorkers;
        }

        private static bool IsWorkerWorkBeforeOrAfter(int day, int shift, int stationId, string workerID)
        {
            DayOfWeek d = (DayOfWeek)day;
            PartOfDay s = (PartOfDay)shift;

            int currentShiftIndex = Shift.GetShiftIndex(d, s);
            int prevShiftIndex = Shift.GetPreviousShiftIndex(d, s);
            int nextShiftIndex = Shift.GetNextShiftIndex(d, s);

            foreach (var station in workSchedule.Template.Shifts[currentShiftIndex].Stations)
            {
                if (station.Workers.Find(x => x.IdNumber == workerID) != null)
                {
                    return true;
                }
            }

            
            if (day == 0 && shift == 0)
            {
                // check if not worked last week saturday night
                if (!string.IsNullOrEmpty(lastSaturdayNightWorkers.Find(x => x == workerID)))
                    return true;
            }
            else
            {
                foreach (var station in workSchedule.Template.Shifts[prevShiftIndex].Stations)
                {
                    if (station.Workers.Find(x => x.IdNumber == workerID) != null)
                    {
                        return true;
                    }
                }
            }

            foreach (var station in workSchedule.Template.Shifts[nextShiftIndex].Stations)
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

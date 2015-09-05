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
        private static int WSID;
        public static WorkSchedule workSchedule;
        public static List<Worker> workers;
        public static List<Station> stations;
        public static List<WorkerConstrains> workersConstrains;
        private static List<string> lastSaturdayNightWorkers;
        public static List<Tuple<int, string>> WorkersStations;
        public static List<SortedScheduleConstrains> sortedStationsConstrains;

        private static int SubmittedWorkers;
        private static int MaxSubmittedWorkers;

        private static Settings algorithmSettings;

        #region Algorithm

        private static void Init()
        {
            WSID = clnt.GetWSID();
            workSchedule = new WorkSchedule(WSID, clnt.GetWeekStartDate());
            workers = clnt.GetWorkers();
            stations = clnt.GetStations(Entities.StationStatus.None);
            workersConstrains = clnt.GetAllWorkersConstrains(workSchedule.WSID);
            lastSaturdayNightWorkers = clnt.GetLastSaturdayNightWorkers(WSID);
            WorkersStations = clnt.GetWorkersStations();
            sortedStationsConstrains = clnt.GetSortedStationConstrains(workSchedule.WSID);
            SubmittedWorkers = 0;
            MaxSubmittedWorkers = 0;
            workSchedule.Capacity = 0;
        }

        public static void GenerateWorkSchedule()
        {
            Init();
            ///TODO: take holidays in consideration
            int scCount = sortedStationsConstrains.Count;

            for (int i = 0; i < scCount; i++)
            {
                int nextSc = GetNextStationsConstrainsIndex();
                SortedScheduleConstrains currentSc = sortedStationsConstrains[nextSc];
                sortedStationsConstrains.RemoveAt(nextSc);

                int sid = currentSc.StationId;
                int day = currentSc.Day;
                int shift = currentSc.ShiftTime;
                if (currentSc.Status == StationStatus.Active && workSchedule.Schedule.Shifts[Shift.GetShiftIndex((DayOfWeek)day, (PartOfDay)shift)].IsActive)
                {
                    int maxWorkers = currentSc.NumberOfWorkers;
                    var fitsWorkers = CalculateWorkersGrade(sid, day, shift, maxWorkers);
                    AddWorkersToSchedule(sid, day, shift, fitsWorkers);
                }
            }

            workSchedule.Capacity = (double)SubmittedWorkers / MaxSubmittedWorkers;

        }

        private static int GetNextStationsConstrainsIndex()
        {
            List<Tuple<int, double>> grades = new List<Tuple<int, double>>(); ;

            for (int i = 0; i < sortedStationsConstrains.Count; i++)
            {
                grades.Add(new Tuple<int, double>(i, CalculateStationsConstrainsGrade(i)));
            }

            // filter all station constrains having max grade
            grades = grades.OrderByDescending(x => x.Item2).ToList()
                           .FindAll(x => x.Item2 == grades[0].Item2);

            // choose randomally station constrain with max grade
            int rndIndex = new Random().Next(0, grades.Count);

            return grades[rndIndex].Item1;
        }

        private static double CalculateStationsConstrainsGrade(int index)
        {
            var currentSc = sortedStationsConstrains[index];
            double grade = 0;

            if (currentSc.ShiftTime == 2)
            {
                grade += 2;
            }

            grade += currentSc.Priority / 5f;

            grade += CalculateProbabilityOfShiftToBeSet(currentSc);


            return grade;
        }

        private static double CalculateProbabilityOfShiftToBeSet(SortedScheduleConstrains current)
        {
            var stationConstrains = sortedStationsConstrains.FindAll(x => x.StationId == current.StationId);
            int needeWorkers = stationConstrains.Sum(x => x.NumberOfWorkers);
            int possibleOptions = 0;

            foreach (var w in workers)
            {
                if (WorkersStations.Any(x => x.Item1 == current.StationId && x.Item2.TrimStart('0') == w.IdNumber.TrimStart('0')))
                {
                    var wcs = workersConstrains.Find(x => x.WorkerID == w.IdNumber);

                    if (wcs != null)
                    {
                        foreach (var sc in stationConstrains)
                        {
                            if (!wcs.Constrains[sc.Day][sc.ShiftTime])
                            {
                                bool workerAlreadyAssigned = false;
                                var assignedShift = workSchedule.Schedule.Shifts[Shift.GetShiftIndex((DayOfWeek)sc.Day, (PartOfDay)sc.ShiftTime)];
                                
                                if (assignedShift != null && assignedShift.Stations != null)
                                {
                                    foreach (var st in assignedShift.Stations)
                                    {
                                        if (st.Workers.Find(x => x.IdNumber == w.IdNumber) != null)
                                        {
                                            workerAlreadyAssigned = true;
                                            break;
                                        }
                                    }
                                }
                                if (!workerAlreadyAssigned)
                                {
                                    possibleOptions += sc.NumberOfWorkers;
                                }
                            }
                        }
                    }
                }
            }

            return (double)possibleOptions / needeWorkers;
        }

        private static void AddWorkersToSchedule(int stationId, int day, int shift, List<string> fitsWorkers)
        {
            List<Worker> fitWorkersList = new List<Worker>();
            foreach (string w in fitsWorkers)
            {
                Worker worker = workers.Find(x => x.IdNumber.TrimStart('0') == w);
                worker.ShiftCounter++;
                fitWorkersList.Add(worker);
            }

            SubmittedWorkers += fitWorkersList.Count;
            

            var station = stations.Find(x => x.Id == stationId);
            MaxSubmittedWorkers += station.NumberOfWorkers;

            Station s = new Station(station);

            s.Workers.AddRange(fitWorkersList);

            workSchedule.Schedule
                        .Shifts[Shift.GetShiftIndex((DayOfWeek)day, (PartOfDay)shift)]
                        .Stations.Add(s);

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

                // check type
                Worker worker = workers.Find(x => x.IdNumber.TrimStart('0') == w);

                // check for meshek - work only in the morning
                if (worker.Type == WorkerTypes.Meshek && shift > 0)
                {
                    continue;
                }
                else if (worker.Type == WorkerTypes.Meshek)
                {
                    grade += 5;
                }
                // check for garin - not working on wednesday at noon
                if (worker.Type == WorkerTypes.Garin && shift == 1 && day == 3)
                {
                    continue;
                }
                else if (worker.Type == WorkerTypes.Garin)
                {
                    grade += 5;
                }


                if (worker.ShiftCounter >= 5)
                {
                    continue;
                }
                else
                {
                    grade += 1 - worker.ShiftCounter / 5f; 
                }

                if (shift == 2 && worker.NightsCounter >= 7)
                {
                    continue;
                }
                else
                {
                    grade += 1 - worker.NightsCounter / 7f;
                }

                WorkerConstrains workerConstrains = workersConstrains.Find(x => x.WorkerID.TrimStart('0') == w);
                try
                {
                    if (workerConstrains.Constrains[day][shift] == true || IsWorkerWorkBeforeOrAfter(day, shift, stationId, w))
                {
                    continue;
                }

                }
                catch
                {

                }


                grades.Add(new Tuple<string, double>(w, grade));
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

            foreach (var station in workSchedule.Schedule.Shifts[currentShiftIndex].Stations)
            {
                if (station.Workers != null && station.Workers.Find(x => x.IdNumber.TrimStart('0') == workerID) != null)
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
                foreach (var station in workSchedule.Schedule.Shifts[prevShiftIndex].Stations)
                {
                    if (station.Workers != null && station.Workers.Find(x => x.IdNumber.TrimStart('0') == workerID) != null)
                    {
                        return true;
                    }
                }
            }

            foreach (var station in workSchedule.Schedule.Shifts[nextShiftIndex].Stations)
            {
                if (station.Workers != null && station.Workers.Find(x => x.IdNumber.TrimStart('0') == workerID) != null)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Algorithm
    }
}

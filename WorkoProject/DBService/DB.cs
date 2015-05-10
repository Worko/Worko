using DBAgent;
using DBServiceInterfaces;
using Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DBService
{
    public class DB : IDB
    {
        #region Workers

        public int AddWorker(Worker worker)
        {
            return Invoker.AddWorker(worker);
        }

        public Worker Login(string id, string pass)
        {
            return Invoker.Login(id, pass);
        }

        public Worker AutoLogin(string id)
        {
            return Invoker.AutoLogin(id);
        }

        public List<Worker> GetWorkers()
        {
            return Invoker.GetWorkers();
        }

        public int UpdateWorker(Worker worker)
        {
            return Invoker.UpdateWorker(worker);
        }

        public void DeleteWorker(string workerId)
        {
           Invoker.DeleteWorker(workerId);
        }

        public int AddWorkerType(string TypeName)
        {
            return Invoker.AddWorkerType(TypeName);
        }

        public List<WorkerType> GetWorkerTypes()
        {
            return Invoker.GetWorkerTypes();
        }

        #endregion

        #region Stations

        public List<Tuple<int, string>> GetWorkersStations()
        {
            return Invoker.GetWorkersStations();
        }

        public List<Station> GetStations(StationStatus status)
        {
            return Invoker.GetStations(status);
        }

        public int AddStation(Station station)
        {
            return Invoker.AddStation(station);
        }
        
        public int UpdateStation(Station station)
        {
            return Invoker.UpdateStation(station);
        }

        public void LinkWorkerToStation(int workerID, int stationID)
        {
            Invoker.LinkWorkerToStation(workerID, stationID);
        }

        public void UnLinkWorkerToStation(int workerID, int stationID)
        {
            Invoker.UnLinkWorkerToStation(workerID, stationID);
        }

        public List<string> GetWorkersByStationID(int stationID)
        {
            return Invoker.GetWorkersByStationID(stationID);
        }

        public void DeleteStation(string stationId)
        {
            Invoker.DeleteStation(stationId);
        }

        #endregion

        #region WorkSchedule

        public int AddStationConstrains(int stationId, int wsid, int day, int shiftTime, int status, int numOfWorkers, int priority)
        {
            return Invoker.AddStationConstrains(stationId, wsid, day, shiftTime, status, numOfWorkers, priority);
        }

        public int RemoveStationConstrains(int stationId, int wsid, int day, int shiftTime)
        {
            return Invoker.RemoveStationConstrains(stationId, wsid, day, shiftTime);
        }

        public int RemoveAllStationConstrains(int wsid)
        {
            return Invoker.RemoveAllStationConstrains(wsid);
        }

        public int AddWorkerConstrains(ShiftsConstrains shiftsConstrains)
        {
            return Invoker.AddWorkerConstrains(shiftsConstrains);
        }

        public int GetWSID(int backWeeks = 0)
        {
            return Invoker.GetWSID(backWeeks);
        }

        public DateTime GetWeekStartDate(int backWeeks = 0)
        {
            return Invoker.GetWeekStartDate(backWeeks);
        }

        public List<bool> GetWorkerConstrains(string workerId, int wsid)
        {
            return Invoker.GetWorkerConstrains(workerId, wsid);
        }

        public List<ScheduleConstrains> GetStationConstrains(int wsid)
        {
            return Invoker.GetStationConstrains(wsid);
        }

        public List<SortedScheduleConstrains> GetSortedStationConstrains(int wsid)
        {
            List<SortedScheduleConstrains> list = new List<SortedScheduleConstrains>();
            var stations = GetStations(StationStatus.Active);
            var res =  Invoker.GetSortedStationConstrains(wsid);

            foreach (var s in stations)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var constrain = res.Find(x => x.StationId == s.Id && x.Day == i && x.ShiftTime == j);
                        if (constrain == null)
                        {
                            constrain = new SortedScheduleConstrains()
                            {
                                Day = i,
                                ShiftTime = j,
                                Priority = s.Priority,
                                StationId = s.Id,
                                Status = s.Status,
                                NumberOfWorkers = s.NumberOfWorkers
                            };
                        }
                        list.Add(constrain);
                    }
                }
            }

            list = list.OrderByDescending(x => x.ShiftTime)
                        .ThenByDescending(x => x.Priority).ToList()
                        .FindAll(x => x.Status == StationStatus.Active);

            return list;
        }

        public List<WorkerConstrains> GetAllWorkersConstrains(int wsid)
        {
            return Invoker.GetAllWorkersConstrains(wsid);
        }

        public void SetNextWeek()
        {
            Invoker.SetNextWeek();
        }

        public void CreateWorkSchedule(WorkSchedule ws)
        {
            Invoker.CreateWorkSchedule(ws);
        }

        public WorkSchedule GetWeeklySchedule(int wsid)
        {
            return Invoker.GetWeeklySchedule(wsid);
        }

        #endregion

        #region Requests

        public int AddWorkerRequest(Request request)
        {
            return Invoker.AddWorkerRequest(request);
        }

        public List<Request> GetUnreadWorkersRequests()
        {
            return Invoker.GetUnreadWorkersRequests();
        }

        public void UpdateWorkerRequest(string requestId)
        {
            Invoker.UpdateWorkerRequest(requestId);
        }
        #endregion

        #region Shifts

        public List<string> GetLastSaturdayNightWorkers(int wsid)
        {
            return Invoker.GetLastSaturdayNightWorkers(wsid);
        }

        #endregion
    }
}

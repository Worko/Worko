using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DBServiceInterfaces
{
    [ServiceContract]
    public interface IDB
    {
        #region Workers
        
        [OperationContract]
        int AddWorker(Worker worker);

        [OperationContract]
        Worker Login(string id, string pass);

        [OperationContract]
        Worker AutoLogin(string id);

        [OperationContract]
        List<Worker> GetWorkers();

        [OperationContract]
        int UpdateWorker(Worker worker);

        [OperationContract]
        void DeleteWorker(string worker);
        #endregion

        #region Stations
        [OperationContract]
        List<Tuple<int, string>> GetWorkersStations();

        [OperationContract]
        List<Station> GetStations(StationStatus status);

        [OperationContract]
        int AddStation(Station station);

        [OperationContract]
        int UpdateStation(Station station);

        [OperationContract]
        void LinkWorkerToStation(int workerID, int stationID);

        [OperationContract]
        void UnLinkWorkerToStation(int workerID, int stationID);

        [OperationContract]
        List<string> GetWorkersByStationID(int stationID);

        [OperationContract]
        void DeleteStation(string stationId);
        #endregion

        #region WorkSchedule

        [OperationContract]
        int AddStationConstrains(int stationId, int wsid, int day, int shiftTime, int status, int numOfWorkers, int priority);

        [OperationContract]
        int RemoveStationConstrains(int stationId, int wsid, int day, int shiftTime);

        [OperationContract]
        int RemoveAllStationConstrains(int wsid);

        [OperationContract]
        int AddWorkerConstrains(ShiftsConstrains shiftsConstrains);

        [OperationContract]
        int GetWSID(int backWeeks = 0);

        [OperationContract]
        DateTime GetWeekStartDate(int backWeeks = 0);

        [OperationContract]
        List<bool> GetWorkerConstrains(string workerId, int wsid);

        [OperationContract]
        List<ScheduleConstrains> GetStationConstrains(int wsid);

        [OperationContract]
        List<SortedScheduleConstrains> GetSortedStationConstrains(int wsid);

        [OperationContract]
        List<WorkerConstrains> GetAllWorkersConstrains(int wsid);

        [OperationContract]
        void SetNextWeek();

        [OperationContract]
        WorkSchedule GetWeeklySchedule(int wsid);

        [OperationContract]
        void CreateWorkSchedule(WorkSchedule ws);
        #endregion

        #region Requests

        [OperationContract]
        int AddWorkerRequest(Request request);

        [OperationContract]
        List<Request> GetUnreadWorkersRequests();

        [OperationContract]
        void UpdateWorkerRequest(string requestId);

        #endregion

        #region Shifts

        [OperationContract]
        List<string> GetLastSaturdayNightWorkers(int wsid);

        #endregion
    }
}

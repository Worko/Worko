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
        int AddWorker(WorkerDC worker);

        [OperationContract]
        WorkerDC Login(string id, string pass);

        [OperationContract]
        WorkerDC AutoLogin(string id);

        [OperationContract]
        List<WorkerDC> GetWorkers();

        [OperationContract]
        int UpdateWorker(WorkerDC worker);

        [OperationContract]
        void DeleteWorker(string worker);
        #endregion

        #region Stations

        [OperationContract]
        List<StationDC> GetStations(StationStatus status);

        [OperationContract]
        int AddStation(StationDC station);

        [OperationContract]
        int UpdateStation(StationDC station);

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
        int AddWorkerConstrains(ShiftsConstrainsDC shiftsConstrains);

        [OperationContract]
        int GetWSID(int backWeeks = 0);

        [OperationContract]
        List<bool> GetWorkerConstrains(string workerId, int wsid);

        [OperationContract]
        List<ScheduleConstrainsDC> GetStationConstrains(int wsid);

        [OperationContract]
        List<SortedScheduleConstrainsDC> GetSortedStationConstrains(int wsid);

        [OperationContract]
        List<WorkerConstrains> GetAllWorkersConstrains(int wsid);

        #endregion

        #region Requests

        [OperationContract]
        int AddWorkerRequest(RequestDC request);

        #endregion
    }
}

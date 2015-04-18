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

        int AddStationConstrains(int stationId, int wsid, int day, int shiftTime);

        int RemoveStationConstrains(int stationId, int wsid, int day, int shiftTime);

        int AddWorkerConstrains(ShiftsConstrainsDC shiftsConstrains);

        int GetWSID(int backWeeks = 0);

        List<bool> GetWorkerConstrains(string workerId, int wsid);

        List<ScheduleConstrainsDC> GetStationConstrains(int wsid);

        #endregion

        #region Requests

        int AddWorkerRequest(RequestDC request);

        #endregion
    }
}

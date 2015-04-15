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

        public int AddWorker(WorkerDC worker)
        {
            return Invoker.AddWorker(worker);
        }

        public WorkerDC Login(string id, string pass)
        {
            return Invoker.Login(id, pass);
        }

        public WorkerDC AutoLogin(string id)
        {
            return Invoker.AutoLogin(id);
        }

        public List<WorkerDC> GetWorkers()
        {
            return Invoker.GetWorkers();
        }

        public int UpdateWorker(WorkerDC worker)
        {
            return Invoker.UpdateWorker(worker);
        }

        public void DeleteWorker(string workerId)
        {
           Invoker.DeleteWorker(workerId);
        }

        #endregion

        #region Stations

        public List<StationDC> GetStations(StationStatus status)
        {
            return Invoker.GetStations(status);
        }

        public int AddStation(StationDC station)
        {
            return Invoker.AddStation(station);
        }
        
        public int UpdateStation(StationDC station)
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
        #endregion
    }
}

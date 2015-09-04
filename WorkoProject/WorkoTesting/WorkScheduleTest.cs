using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkoProject.Utils;
using Entities;

namespace WorkoTesting
{
    [TestClass]
    public class WorkScheduleTest
    {
        private WorkSchedule ws;

        public WorkScheduleTest()
        {
            DBAgent.Invoker.OpenConnection();
            WorkoAlgorithm.GenerateWorkSchedule();
            ws = WorkoAlgorithm.workSchedule;
        }

        /// <summary>
        /// Check if the work schedule efficiency above 80 percent
        /// </summary>
        [TestMethod]
        public void EfficiencyTest()
        {
            double expected = 0.8;
            Assert.IsTrue(ws.Capacity > expected, "Efficiency: " + ws.Capacity);
        }

        /// <summary>
        /// Check if all the workers work only at the stations are linked to them
        /// </summary>
        [TestMethod]
        public void WorkerAssignedInLinkedStationOnly()
        {
            var workers = WorkoAlgorithm.workers;
            var shifts = ws.Schedule.Shifts;

            int numOfWorkersToCheck = workers.Count;
            int numOfSuccess = 0;

            foreach (var worker in workers)
            {
                bool success = true;
                foreach (var shift in shifts)
                {
                    foreach (var station in shift.Stations)
                    {
                        var tmpWorker = station.Workers.Find(x => x.IdNumber.TrimStart('0') == worker.IdNumber.TrimStart('0'));
                        if (tmpWorker != null)
                        {
                            var workerStation = WorkoAlgorithm.WorkersStations.Find(x => x.Item1 == station.Id && x.Item2.TrimStart('0') == worker.IdNumber.TrimStart('0'));
                            if (workerStation == null)
                            {
                                success = false;
                                break;
                            }
                        }
                    } // foreach stations

                    if (!success)
                    {
                        break;
                    }

                }// foreach shifts

                if (success)
                {
                    numOfSuccess++;
                }

            }// foreach workers

            Assert.IsTrue(numOfSuccess == numOfWorkersToCheck, "Success: " + numOfSuccess + " of " + numOfWorkersToCheck);
        }

        /// <summary>
        /// Check that workers don't assigned to more than 5 shifts in week
        /// </summary>
        [TestMethod]
        public void MaxFiveShifts()
        {
            var workers = WorkoAlgorithm.workers;
            var shifts = ws.Schedule.Shifts;

            int numOfWorkersToCheck = workers.Count;
            int numOfSuccess = 0;

            foreach (var worker in workers)
            {
                int shiftCount = 0;
                foreach (var shift in shifts)
                {
                    foreach (var station in shift.Stations)
                    {
                        var tmpWorker = station.Workers.Find(x => x.IdNumber.TrimStart('0') == worker.IdNumber.TrimStart('0'));
                        if (tmpWorker != null)
                        {
                            shiftCount++;
                        }
                    } // foreach stations

                }// foreach shifts
                if (shiftCount > 5)
                {
                    break;
                }
                else
                {
                    numOfSuccess++;
                }
            }// foreach workers

            Assert.IsTrue(numOfSuccess == numOfWorkersToCheck, "Success: " + numOfSuccess + " of " + numOfWorkersToCheck);
        }

        /// <summary>
        /// Check that workers don't work at shifts that assigned as constrains
        /// </summary>
        [TestMethod]
        public void WorkerConstrains()
        {
            var workers = WorkoAlgorithm.workers;
            var shifts = ws.Schedule.Shifts;
            var workerConstrains = WorkoAlgorithm.workersConstrains;

            int numOfWorkersToCheck = workers.Count;
            int numOfSuccess = 0;

            foreach (var worker in workers)
            {
                bool success = true;
                var wc = workerConstrains.Find(x => x.WorkerID.TrimStart('0') == worker.IdNumber.TrimStart('0'));
                foreach (var shift in shifts)
                {
                    foreach (var station in shift.Stations)
                    {
                        var tmpWorker = station.Workers.Find(x => x.IdNumber.TrimStart('0') == worker.IdNumber.TrimStart('0'));
                        if (tmpWorker != null)
                        {
                            if (wc.Constrains[(int)shift.Day][(int)shift.Part])
                            {
                                success = false;
                                break;
                            }
                        }
                    } // foreach stations

                    if (!success)
                    {
                        break;
                    }
                }// foreach shifts

                if (success)
                {
                    numOfSuccess++;
                }
            }// foreach workers

            Assert.IsTrue(numOfSuccess == numOfWorkersToCheck, "Success: " + numOfSuccess + " of " + numOfWorkersToCheck);
        }

        /// <summary>
        /// Check that workers assigned at active shifts only
        /// </summary>
        [TestMethod]
        public void ActiveShifts()
        {
            var shifts = ws.Schedule.Shifts;
            bool success = true;

            foreach (var shift in shifts)
            {
                if (!shift.IsActive)
                {
                    foreach (var station in shift.Stations)
                    {
                        if (station.Workers.Count > 0)
                        {
                            success = false;
                        }
                    } // foreach stations
                }

                if (!success)
                {
                    break;
                }
            }// foreach shifts

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Check that workers assigned at active stations only
        /// </summary>
        [TestMethod]
        public void ActiveStations()
        {
            var shifts = ws.Schedule.Shifts;
            bool success = true;

            foreach (var shift in shifts)
            {
                foreach (var station in shift.Stations)
                {
                    if (station.Workers.Count > 0 && station.Status != StationStatus.Active)
                    {
                        success = false;
                    }
                } // foreach stations

                if (!success)
                {
                    break;
                }
            }// foreach shifts

            Assert.IsTrue(success);
        }
    }
}

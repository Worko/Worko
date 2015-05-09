using Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAgent
{
    public class Invoker
    {
        private static SqlConnection con;
        private static SqlCommand cmd;
        private static SqlDataAdapter sda;
        private static SqlDataReader reader;
        private static SqlParameter sqlParm;
        private static bool open, IsInit = false;

        /// <summary>
        /// Constructor
        /// </summary>
        private static void Init()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["WorkoDB"].ConnectionString;
            con = new SqlConnection(connectionString);
            cmd = new SqlCommand();
            sda = new SqlDataAdapter();
            IsInit = true;
        }

        /// <summary>
        /// Open a connection to the Data Base
        /// </summary>
        /// <returns>True if success , False if not</returns>
        public static bool OpenConnection()
        {
            try
            {
                if (!IsInit)
                {
                    Init();
                }
                if (open)
                    con.Close();
                con.Open();
                open = true;
                return true;
            }
            catch (Exception)
            {
                con.Close();
                open = false;
                return false;
            }
        }

        /// <summary>
        /// Close the connection to the Data Base
        /// </summary>
        public static void CloseConnection()
        {
            if (open)
            {
                con.Close();
                open = false;
            }
        }

        /// <summary>
        /// Get the data from the database for the given spu command
        /// </summary>
        /// <param name="spName">Stored Procedure function</param>
        /// <param name="var">Optional - contains user ID </param>
        /// <param name="var2">Optional - contains date </param>
        /// <param name="search">Optional - determines if it's search option</param>
        /// <param name="search">Optional - return id by email</param>
        /// <returns>DataSet</returns>
        private static DataSet GetDataSet(string spName, List<Tuple<string, object>> vars = null)
        {
            // close connection first (if open)
            CloseConnection();
            cmd = new SqlCommand(spName, con);
            cmd.CommandType = CommandType.StoredProcedure;

            if (vars != null && vars.Count > 0)
            {
                foreach (var v in vars)
                {
                    cmd.Parameters.AddWithValue("@" + v.Item1, v.Item2);
                }
            }

            DataSet ds = new DataSet();
            sda.SelectCommand = cmd;
            // fill the dataset with values from the data base
            sda.Fill(ds);
            //return the data set
            return ds;
        }





        #region Login
        /// <summary>
        /// Check id and password with database
        /// </summary>
        /// <param name="id">User ID Number</param>
        /// <param name="pass">User Password</param>
        /// <returns>SqlDataReader - Result from sp</returns>
        public static Worker Login(string id, string pass)
        {
            OpenConnection();
            EncryptPassword encryptPass = new EncryptPassword();
            encryptPass.HashedPass = pass;

            // create new StoredProcedure command
            cmd = new SqlCommand("sp_Login", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the id and pass parameters
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@pass", encryptPass.HashedPass);

            // return the reader
            reader = cmd.ExecuteReader();

            Worker w = null;
            if (reader.Read())
            {
                w = new Worker()
                {
                    IdNumber = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    Email = reader.GetString(4),
                    Phone = reader.GetString(5),
                    IsAdmin = reader.GetBoolean(6)
                };

            }
            CloseConnection();
            return w;
        }

        public static Worker AutoLogin(string id)
        {
            OpenConnection();

            // create new StoredProcedure command
            cmd = new SqlCommand("sp_AutoLogin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the id and pass parameters
            cmd.Parameters.AddWithValue("@id", id);

            // return the reader
            reader = cmd.ExecuteReader();

            Worker w = null;
            if (reader.Read())
            {
                w = new Worker()
                {
                    IdNumber = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    Email = reader.GetString(4),
                    Phone = reader.GetString(5),
                    IsAdmin = reader.GetBoolean(6)
                };

            }
            CloseConnection();
            return w;
        }

        #endregion

        #region Workers

        public static int AddWorker(Worker worker)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_AddWorker", con);
            cmd.CommandType = CommandType.StoredProcedure;

            EncryptPassword en = new EncryptPassword();
            en.HashedPass = worker.Password;

            // add the parameters
            cmd.Parameters.AddWithValue("@IdNumber", worker.IdNumber);
            cmd.Parameters.AddWithValue("@FirstName", worker.FirstName);
            cmd.Parameters.AddWithValue("@LastName", worker.LastName);
            cmd.Parameters.AddWithValue("@Password", en.HashedPass);
            cmd.Parameters.AddWithValue("@Phone", worker.Phone != null ? worker.Phone : "");
            cmd.Parameters.AddWithValue("@Email", worker.Email != null ? worker.Email : "");
            cmd.Parameters.AddWithValue("@Picture", worker.Picture != null ? worker.Picture : "");
            cmd.Parameters.AddWithValue("@Type", worker.Type);

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            int res = (int)cmd.Parameters["@res"].Value;
            CloseConnection();
            return res;
        }


        public static List<Worker> GetWorkers()
        {
            var ds = GetDataSet("sp_GetWorkers");
            List<Worker> workers = new List<Worker>();

            int wsid = GetWSID();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var row = ds.Tables[0].Rows[i];
                workers.Add(new Worker()
                {
                    
                    IdNumber = (string)row["IdNumber"],
                    FirstName = (string)row["FirstName"],
                    LastName = (string)row["LastName"],
                    Email = (string)row["Email"],
                    Phone = (string)row["Phone"],
                    IsAdmin = (bool)row["IsAdmin"],
                    Picture = (string)row["Picture"],
                    Type = (WorkerType)row["Type"],
                    ShiftCounter = 0,
                   NightsCounter = GetWorkerNightShiftCount(wsid, int.Parse((string)row["IdNumber"]))
                });
            }

            return workers;
        }

        private static int GetWorkerNightShiftCount(int wsid, int workerId)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_GetWorkerNightShiftCount", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@WSID", wsid);
            cmd.Parameters.AddWithValue("@workerId", workerId);

            // return the reader
            reader = cmd.ExecuteReader();
            int count = 0;
            if (reader.Read())
            {
                count = reader.GetInt32(0);
            }

            CloseConnection();
            return count;
        }


        public static int UpdateWorker(Worker worker)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_UpdateWorker", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@IdNumber", worker.IdNumber);
            cmd.Parameters.AddWithValue("@Fname", worker.FirstName);
            cmd.Parameters.AddWithValue("@Lname", worker.LastName);
            cmd.Parameters.AddWithValue("@Phone", worker.Phone);
            cmd.Parameters.AddWithValue("@Email", worker.Email);
            cmd.Parameters.AddWithValue("@Picture", string.IsNullOrEmpty(worker.Picture) ? string.Empty : worker.Picture);
            cmd.Parameters.AddWithValue("@Type", worker.Type);

            // need to add this sometime...
            //cmd.Parameters.AddWithValue("@Password", worker.Password);

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            int res = (int)cmd.Parameters["@res"].Value;
            CloseConnection();
            return res;
        }


        public static void DeleteWorker(string workerId)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_DeleteWorker", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@IdNumber", workerId);

            cmd.ExecuteNonQuery();
        }



        public static void LinkWorkerToStation(int workerID, int stationID)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_LinkWorkerToStation", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@workerID", workerID);
            cmd.Parameters.AddWithValue("@stationID", stationID);

            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public static void UnLinkWorkerToStation(int workerID, int stationID)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_UnLinkWorkerToStation", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@workerID", workerID);
            cmd.Parameters.AddWithValue("@stationID", stationID);

            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public static List<string> GetWorkersByStationID(int stationID)
        {
            var args = new List<Tuple<string, object>>();
            args.Add(new Tuple<string, object>("stationID", stationID));
            var ds = GetDataSet("sp_GetWorkersByStationID", args);
            List<string> workers = new List<string>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                workers.Add(ds.Tables[0].Rows[i][0].ToString());
            }

            return workers;
        }
        #endregion

        #region Stations

        public static List<Tuple<int, string>> GetWorkersStations()
        {
            var ds = GetDataSet("sp_GetWorkersStations");

            List<Tuple<int, string>> ws = new List<Tuple<int, string>>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var row = ds.Tables[0].Rows[i];
                ws.Add(new Tuple<int, string>((int)row["StationId"], row["WorkerId"].ToString()));
            }

            return ws;
        }

        public static List<Station> GetStations(StationStatus status = StationStatus.None)
        {
            List<Tuple<string, object>> args = new List<Tuple<string, object>>();
            args.Add(new Tuple<string, object>("Status", (int)status));
            var ds = GetDataSet("sp_GetStations", args);

            List<Station> stations = new List<Station>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                stations.Add(new Station()
                {
                    Id = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                    Name = ds.Tables[0].Rows[i][1].ToString(),
                    Description = ds.Tables[0].Rows[i][2].ToString(),
                    Status = (StationStatus)int.Parse(ds.Tables[0].Rows[i][3].ToString()),
                    Priority = int.Parse(ds.Tables[0].Rows[i][4].ToString()),
                    NumberOfWorkers = int.Parse(ds.Tables[0].Rows[i][5].ToString())
                });
            }

            return stations;
        }

        public static int AddStation(Station station)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_AddStation", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@StationName", station.Name);
            cmd.Parameters.AddWithValue("@Description", station.Description);
            cmd.Parameters.AddWithValue("@Status", (int)station.Status);

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            int res = (int)cmd.Parameters["@res"].Value;
            CloseConnection();
            return res;
        }

        public static int UpdateStation(Station station)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_UpdateStation", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@Id", station.Id);
            cmd.Parameters.AddWithValue("@StationName", station.Name);
            cmd.Parameters.AddWithValue("@Description", station.Description);
            cmd.Parameters.AddWithValue("@Status", (int)station.Status);
            cmd.Parameters.AddWithValue("@NumberOfWorkers", station.NumberOfWorkers);
            cmd.Parameters.AddWithValue("@Priority", station.Priority);

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            int res = (int)cmd.Parameters["@res"].Value;
            CloseConnection();
            return res;
        }


        public static void DeleteStation(string stationId)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_DeleteStation", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@StationId", stationId);

            cmd.ExecuteNonQuery();
        }
        #endregion

        #region Constrains
        public static List<WorkerConstrains> GetAllWorkersConstrains(int wsid)
        {
            List<WorkerConstrains> list = new List<WorkerConstrains>();
            
            try
            {
                List<Tuple<string, object>> args = new List<Tuple<string, object>>();
                args.Add(new Tuple<string, object>("WSID", wsid));
                var ds = GetDataSet("sp_GetAllWorkersConstrains", args);

                string curWorker = "";
                int index = -1;

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];

                    int day = (int)row["Day"];
                    int shift = (int)row["ShiftTime"];
                    string id = (string)row["WorkerId"];

                    if (curWorker != id)
                    {
                        index++;
                        curWorker = id;
                        var wc = new WorkerConstrains();
                        wc.WorkerID = id;
                        wc.Constrains[day][shift] = true;
                        list.Add(wc);
                    }
                    else
                    {
                        list[index].Constrains[day][shift] = true;
                    }
                }

            }
            catch { }

            return list;
        }


        public static List<bool> GetWorkerConstrains(string workerId, int wsid)
        {
            List<Tuple<string, object>> args = new List<Tuple<string, object>>();
            args.Add(new Tuple<string, object>("WorkerId", workerId));
            args.Add(new Tuple<string, object>("WSID", wsid));
            var ds = GetDataSet("sp_GetWorkerConstrains", args);

            List<bool> wc = new List<bool>();
            for (int i = 0; i < 21; i++)
            {
                wc.Add(false);
            }

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int day = int.Parse(ds.Tables[0].Rows[i][3].ToString());
                int part = int.Parse(ds.Tables[0].Rows[i][4].ToString());
                wc[Shift.GetShiftIndex((DayOfWeek)day, (PartOfDay)part)] = true;
            }
            
            return wc;
        }

        public static int RemoveWorkerConstrains(string workerId, int wsid)
        {
            int res = 0;
            try
            {
                OpenConnection();
                // create new StoredProcedure command
                cmd = new SqlCommand("sp_RemoveWorkerConstrains", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // add the parameters
                cmd.Parameters.AddWithValue("@WorkerId", workerId);
                cmd.Parameters.AddWithValue("@WSID", wsid);

                cmd.ExecuteNonQuery();
                CloseConnection();
                res = 1;
            }
            catch 
            {
                res = 0;
            }
            return res;
        }

        public static void SetNextWeek()
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_SetNextWeekSchedule", con);
            cmd.CommandType = CommandType.StoredProcedure;

            DateTime date = DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek);

            // add the parameters
            cmd.Parameters.AddWithValue("@Date", date.Date);

            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public static int GetWSID(int backWeeks = 0)
        {
            List<Tuple<string, object>> args = new List<Tuple<string, object>>();
            args.Add(new Tuple<string, object>("Week", backWeeks));
            var ds = GetDataSet("sp_GetNextWeekSchedule", args);

            try
            {
                int res = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                return res;
            }
            catch (Exception)
            {
                SetNextWeek();
                return GetWSID();
            }
        }

        public static DateTime GetWeekStartDate(int backWeeks = 0)
        {
            List<Tuple<string, object>> args = new List<Tuple<string, object>>();
            args.Add(new Tuple<string, object>("Week", backWeeks));
            var ds = GetDataSet("sp_GetNextWeekSchedule", args);

            try
            {
                return (DateTime)ds.Tables[0].Rows[0][1];
            }
            catch (Exception)
            {
                SetNextWeek();
                return GetWeekStartDate();
            }
        }

        public static WorkSchedule GetWeeklySchedule(int wsid)
        {
            List<Tuple<string, object>> args = new List<Tuple<string, object>>();
            args.Add(new Tuple<string, object>("WSID", wsid));
            var ds = GetDataSet("sp_GetWeeklySchedule", args);

            WorkSchedule ws = new WorkSchedule(wsid, GetWeekStartDate());
            var workers = GetWorkers();
            var stations = GetStations();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var row = ds.Tables[0].Rows[i];
                int shift = (int)row["ShiftTime"];
                int day = (int)row["Day"];
                int stationId = (int)row["StationId"];
                string workerId = row["WorkerId"].ToString();
                var worker = workers.Find(x => x.IdNumber.TrimStart('0') == workerId);
                var station = stations.Find(x => x.Id == stationId);

                int shiftIndex = Shift.GetShiftIndex((DayOfWeek)day, (PartOfDay)shift);

                var currentStation = ws.Template.Shifts[shiftIndex].Stations.Find(x => x.Id == station.Id);
                if (currentStation == null)
                {
                    currentStation = new Station(station);
                    ws.Template.Shifts[shiftIndex].Stations.Add(currentStation);
                }

                currentStation.Workers.Add(worker);
                
                
            }

            return ws;
        }

        public static int AddWorkerConstrains(ShiftsConstrains shiftsConstrains)
        {
            if (RemoveWorkerConstrains(shiftsConstrains.WorkerId, shiftsConstrains.WSID) == 1)
            {
                OpenConnection();
                for (int i = 0; i < shiftsConstrains.Constrains.Count; i++)
                {
                    if (shiftsConstrains.Constrains[i])
                    {
                        int part = i / 7;
                        int day = i - 7 * part;

                        // create new StoredProcedure command
                        cmd = new SqlCommand("sp_AddWorkerConstrains", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        // add the parameters
                        cmd.Parameters.AddWithValue("@WSID", shiftsConstrains.WSID);
                        cmd.Parameters.AddWithValue("@WorkerId", shiftsConstrains.WorkerId);
                        cmd.Parameters.AddWithValue("@Day", day);
                        cmd.Parameters.AddWithValue("@ShiftTime", part);


                        cmd.ExecuteNonQuery();
                    }
                }

                CloseConnection();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int AddWorkerRequest(Request request)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_AddWorkerRequest", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@WorkerId", request.WorkerId);
            cmd.Parameters.AddWithValue("@Date", request.Date);
            cmd.Parameters.AddWithValue("@Title", request.Title);
            cmd.Parameters.AddWithValue("@Content", request.Content);

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            int res = (int)cmd.Parameters["@res"].Value;
            CloseConnection();
            return res;
        }



        public static int RemoveAllStationConstrains(int wsid)
        {
            try
            {
                OpenConnection();
                // create new StoredProcedure command
                cmd = new SqlCommand("sp_RemoveAllStationConstrains", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // add the parameters
                cmd.Parameters.AddWithValue("@WSID", wsid);

                cmd.ExecuteNonQuery();
                CloseConnection();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public static int RemoveStationConstrains(int stationId, int wsid, int day, int shiftTime)
        {
            try
            {
                OpenConnection();
                // create new StoredProcedure command
                cmd = new SqlCommand("sp_RemoveStationConstrains", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // add the parameters
                cmd.Parameters.AddWithValue("@StationId", stationId);
                cmd.Parameters.AddWithValue("@WSID", wsid);
                cmd.Parameters.AddWithValue("@Day", day);
                cmd.Parameters.AddWithValue("@ShiftTime", shiftTime);

                cmd.ExecuteNonQuery();
                CloseConnection();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public static int AddStationConstrains(int stationId, int wsid, int day, int shiftTime, int status, int numOfWorkers, int priority)
        {

            try
            {
                OpenConnection();

                // create new StoredProcedure command
                cmd = new SqlCommand("sp_AddStationConstrains", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // add the parameters
                cmd.Parameters.AddWithValue("@WSID", wsid);
                cmd.Parameters.AddWithValue("@StationId", stationId);
                cmd.Parameters.AddWithValue("@Day", day);
                cmd.Parameters.AddWithValue("@ShiftTime", shiftTime);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@NumberOfWorkers", numOfWorkers);
                cmd.Parameters.AddWithValue("@Priority", priority);

                cmd.ExecuteNonQuery();

                CloseConnection();
                return 1;
            }
            catch
            {
                return 0;
            }
        }


        public static List<SortedScheduleConstrains> GetSortedStationConstrains(int wsid)
        {
            List<SortedScheduleConstrains> list = new List<SortedScheduleConstrains>();

            try
            {
                List<Tuple<string, object>> args = new List<Tuple<string, object>>();
                args.Add(new Tuple<string, object>("WSID", wsid));
                var ds = GetDataSet("sp_GetSortedStationConstrains", args);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];

                    int day = (int)row["Day"];
                    int shift = (int)row["ShiftTime"];
                    int id = (int)row["StationId"];
                    int priority = (int)row["Priority"];
                    int numberOfWorkers = (int)row["NumberOfWorkers"];
                    StationStatus status = (StationStatus)row["Status"];

                    var ssc = new SortedScheduleConstrains();
                    ssc.StationId = id;
                    ssc.Status = status;
                    ssc.Day = day;
                    ssc.NumberOfWorkers = numberOfWorkers;
                    ssc.Priority = priority;
                    ssc.ShiftTime = shift;

                    list.Add(ssc);
                }
            }
            catch { }

            return list;
        }


        public static List<ScheduleConstrains> GetStationConstrains(int wsid)
        {
            List<ScheduleConstrains> list = new List<ScheduleConstrains>();

            try
            {
                List<Tuple<string, object>> args = new List<Tuple<string, object>>();
                args.Add(new Tuple<string, object>("WSID", wsid));
                var ds = GetDataSet("sp_GetStationConstrains", args);
                
                int curStation = -1;
                int index = -1;

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];

                    int day = (int)row["Day"];
                    int shift = (int)row["ShiftTime"];
                    int id = (int)row["StationId"];
                    int priority = (int)row["Priority"];
                    int numberOfWorkers = (int)row["NumberOfWorkers"];
                    StationStatus status = (StationStatus)row["Status"];

                    if (curStation != id)
                    {
                        index++;
                        curStation = (int)row["StationId"];
                        var sc = new ScheduleConstrains();
                        sc.WSID = wsid;
                        sc.StationId = id;
                        sc.Status = status;
                        sc.Constrains = new List<StationConstrains>();
                        sc.Constrains.Add(new StationConstrains()
                        {
                            Day = day,
                            ShiftTime = shift,
                            Priority = priority,
                            NumberOfWorkers = numberOfWorkers,
                            Status = status
                        });
                        list.Add(sc);
                    }
                    else
                    {
                        list[index].Constrains.Add(new StationConstrains()
                        {
                            Day = day,
                            ShiftTime = shift,
                            Priority = priority,
                            NumberOfWorkers = numberOfWorkers,
                            Status = status
                        });
                    }
                }

            }
            catch { }

            return list;
        }

        public static void CreateWorkSchedule(WorkSchedule ws)
        {
            string template = "INSERT INTO WeeklyWorkers (WSID, WorkerId, StationId, Day, ShiftTime)"
                                + "VALUES({0}, {1}, {2}, {3}, {4})";

            string script = string.Empty;

            foreach (var shift in ws.Template.Shifts)
            {
                int day = (int)shift.Day;
                int shiftTime = (int)shift.Part;

                foreach (var station in shift.Stations)
                {
                    int stationId = station.Id;

                    foreach (var worker in station.Workers)
                    {
                        string workerId = worker.IdNumber;
                        script += string.Format(template, ws.WSID, workerId, stationId, day, shiftTime);
                        script += "\n";
                    }
                }
            }

            try
            {
                OpenConnection();
                cmd = new SqlCommand(script, con);
                cmd.ExecuteNonQuery();
            }
            catch
            {
           
            }
            finally
            {
                CloseConnection();
            }

        }

        #endregion

        #region Requests
        public static List<Request> GetUnreadWorkersRequests()
        {
            var ds = GetDataSet("sp_GetUnreadWorkersRequests");
            List<Request> unreadRequests = new List<Request>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var row = ds.Tables[0].Rows[i];

                unreadRequests.Add(new Request()
                {
                    Pkid     = (int)row["pkid"],
                    WorkerId = (string)row["WorkerId"],
                    Date     = (DateTime)row["Date"],
                    Title    = (string)row["Title"],
                    Content  = (string)row["Content"]
                });
            }

            return unreadRequests;
        }

        public static void UpdateWorkerRequest(string requestId)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_UpdateWorkerRequest", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@pkid", int.Parse(requestId));

            cmd.ExecuteNonQuery();
            CloseConnection();
        }
        #endregion

        #region Shifts

        public static List<string> GetLastSaturdayNightWorkers(int wsid)
        {
            List<Tuple<string, object>> args = new List<Tuple<string, object>>();
            args.Add(new Tuple<string, object>("WSID", wsid - 1));
            var ds = GetDataSet("sp_GetSaturdayNightWorkers", args);

            List<string> workers = new List<string>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                workers.Add(ds.Tables[0].Rows[0].ToString());
            }

            return workers;
        }

        #endregion
    }
}
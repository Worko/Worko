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

        public static int AddWorker(WorkerDC worker)
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

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            int res = (int)cmd.Parameters["@res"].Value;
            CloseConnection();
            return res;
        }


        /// <summary>
        /// Check id and password with database
        /// </summary>
        /// <param name="id">User ID Number</param>
        /// <param name="pass">User Password</param>
        /// <returns>SqlDataReader - Result from sp</returns>
        public static WorkerDC Login(string id, string pass)
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

            WorkerDC w = null;
            if (reader.Read())
            {
                w = new WorkerDC()
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

        public static WorkerDC AutoLogin(string id)
        {
            OpenConnection();

            // create new StoredProcedure command
            cmd = new SqlCommand("sp_AutoLogin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the id and pass parameters
            cmd.Parameters.AddWithValue("@id", id);

            // return the reader
            reader = cmd.ExecuteReader();

            WorkerDC w = null;
            if (reader.Read())
            {
                w = new WorkerDC()
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

        public static List<WorkerDC> GetWorkers()
        {
            var ds = GetDataSet("sp_GetWorkers");
            List<WorkerDC> workers = new List<WorkerDC>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                workers.Add(new WorkerDC()
                {
                    IdNumber = ds.Tables[0].Rows[i][0].ToString(),
                    FirstName = ds.Tables[0].Rows[i][1].ToString(),
                    LastName = ds.Tables[0].Rows[i][2].ToString(),
                    Email = ds.Tables[0].Rows[i][3].ToString(),
                    Phone = ds.Tables[0].Rows[i][4].ToString(),
                    IsAdmin = ds.Tables[0].Rows[i][5].ToString() == "true",
                    Picture = ds.Tables[0].Rows[i][6].ToString()
                });
            }

            return workers;
        }


        public static int UpdateWorker(WorkerDC worker)
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



        public static List<StationDC> GetStations(StationStatus status = StationStatus.None)
        {
            List<Tuple<string, object>> args = new List<Tuple<string, object>>();
            args.Add(new Tuple<string, object>("Status", (int)status));
            var ds = GetDataSet("sp_GetStations", args);

            List<StationDC> stations = new List<StationDC>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                stations.Add(new StationDC()
                {
                    Id = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                    Name = ds.Tables[0].Rows[i][1].ToString(),
                    Description = ds.Tables[0].Rows[i][2].ToString(),
                    Status = (StationStatus)int.Parse(ds.Tables[0].Rows[i][3].ToString())
                });
            }

            return stations;
        }

        public static int AddStation(StationDC station)
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

        public static int UpdateStation(StationDC station)
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

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            int res = (int)cmd.Parameters["@res"].Value;
            CloseConnection();
            return res;
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
                wc[ShiftDC.GetShiftIndex((DayOfWeek)day, (PartOfDay)part)] = true;
            }

            return wc;
        }

        public static int RemoveWorkerConstrains(string workerId, int wsid)
        {
            OpenConnection();
            // create new StoredProcedure command
            cmd = new SqlCommand("sp_RemoveWorkerConstrains", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@WorkerId", workerId);
            cmd.Parameters.AddWithValue("@WSID", wsid);

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            int res = (int)cmd.Parameters["@res"].Value;
            CloseConnection();
            return res;
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
                return -1;
            }
        }

        public static int AddWorkerConstrains(ShiftsConstrainsDC shiftsConstrains)
        {
            if (RemoveWorkerConstrains(shiftsConstrains.WorkerId, shiftsConstrains.WSID) == 1)
            {
                int res = 1;

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

                        sqlParm = new SqlParameter("@res", DbType.Int32);
                        sqlParm.Direction = ParameterDirection.Output;
                        // add the result parameter
                        cmd.Parameters.Add(sqlParm);

                        cmd.ExecuteNonQuery();
                        res *= (int)cmd.Parameters["@res"].Value;
                    }
                }

                CloseConnection();
                return res;
            }
            else
            {
                return 0;
            }
        }

        public static int AddWorkerRequest(RequestDC request)
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










        public static bool InsertRequest(string id, string cause, string date)
        {
            // create new StoredProcedure command
            cmd = new SqlCommand("spu_InsertRequest", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@cause", cause);
            cmd.Parameters.AddWithValue("@date", date);

            sqlParm = new SqlParameter("@res", DbType.Int32);
            sqlParm.Direction = ParameterDirection.Output;
            // add the result parameter
            cmd.Parameters.Add(sqlParm);

            cmd.ExecuteNonQuery();
            return (int)cmd.Parameters["@res"].Value == 1;
        }

        /// <summary>
        /// Delete user activity
        /// </summary>
        /// <param name="userId">user id number</param>
        public static void DeleteUserActivity(string pkid)
        {
            // create new StoredProcedure command
            cmd = new SqlCommand("spu_DeleteUserActivity", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@pkid", pkid);

            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// Update user details in database
        /// </summary>
        /// <param name="fn">user first name</param>
        /// <param name="ln">user last name</param>
        /// <param name="id">user Id number</param>
        /// <param name="email">user email</param>
        /// <param name="phone">user phone</param>
        /// <param name="picture">user picture</param>
        /// <param name="pass">user password</param>
        /// <param name="admin">user admin flag</param>
        /// <returns></returns>
        public static bool UpdateUser(string fn, string ln, string id, string email, string phone, string picture, string pass, string admin = "False")
        {
            // create new StoredProcedure command
            cmd = new SqlCommand("spu_UpdateUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters
            cmd.Parameters.AddWithValue("@fn", fn);
            cmd.Parameters.AddWithValue("@ln", ln);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@picture", picture);
            cmd.Parameters.AddWithValue("@pass", pass);
            cmd.Parameters.AddWithValue("@admin", admin);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


    }
}

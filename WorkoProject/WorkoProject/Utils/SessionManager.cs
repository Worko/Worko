using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using WorkoProject.Models;

namespace WorkoProject.Utils
{
    public class SessionManager
    {
        #region consts

        private const string HAS_CONNECTED_USER = "HasConnectedUser";
        private const string HAS_ADMIN_CONNECTED = "HasAdminConnected";
        private const string CONNECTED_USER = "ConnectedUser";

        #endregion

        private static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        public static bool HasConnectedUser
        {
            get 
            {
                return Session[HAS_CONNECTED_USER] != null ? (bool)Session[HAS_CONNECTED_USER] : false;
            }
            set 
            {
                Session[HAS_CONNECTED_USER] = value;
            }
        }

        public static bool HasAdminConnected
        {
            get
            {
                return Session[HAS_ADMIN_CONNECTED] != null ? (bool)Session[HAS_ADMIN_CONNECTED] : false;
            }
            set
            {
                Session[HAS_ADMIN_CONNECTED] = value;
            }
        }

        public static Worker CurrentWorker
        {
            get
            {
                return (Worker)Session[CONNECTED_USER];
            }
            set
            {
                Session[CONNECTED_USER] = value;
            }
        }

    }
}

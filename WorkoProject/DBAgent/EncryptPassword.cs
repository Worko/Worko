using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBAgent
{
    /* 
   * Class for encrypt the password 
   */
    public class EncryptPassword
    {
        private string hashedPass;

        public string HashedPass
        {
            get { return hashedPass.Length > 49 ? hashedPass.Substring(0, 49) : hashedPass; }
            set
            {
                System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
                hashedPass = System.Convert.ToBase64String(sha1.ComputeHash(System.Text.UnicodeEncoding.Unicode.GetBytes(value)));
            }
        }
    }
}
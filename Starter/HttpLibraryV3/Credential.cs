using System;
using Microsoft.SPOT;
using System.Text;
using System.IO;

namespace HttpLibrary
{
    /// <summary>
    /// Credential class for holding the server security parameters
    /// </summary>
    public class Credential
    {
        /// <summary>
        /// Server name
        /// </summary>
        public string ServerOwner;
        /// <summary>
        ///  Authentication username
        /// </summary>
        public string UserName;
        /// <summary>
        /// Authentication password
        /// </summary>
        public string Password;
        /// <summary>
        /// Base64 encrypted password
        /// </summary>
        public string Key;
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="ServerOwner">Server name</param>
        /// <param name="UserName">Authentication username</param>
        /// <param name="Password">Authentication password</param>
        public Credential(string ServerOwner, string UserName, string Password)
        {
            this.ServerOwner = ServerOwner;
            this.UserName = UserName;
            this.Password = Password;
            this.Key = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(UserName + ":" + Password));
        }
        /// <summary>
        /// Reads a saved credential from memory card
        /// </summary>
        /// <returns></returns>
        public static Credential ReadFromFile()
        {
            FileStream fs = new FileStream(@"\SD\" + "NsC" + ".crdn", FileMode.Open, FileAccess.Read);
            StreamReader Reader = new StreamReader(fs);
            string owner = Reader.ReadLine();
            string keeey = Reader.ReadLine();
            Reader.Close();
            fs.Close();
            string[] unpass = new string(UTF8Encoding.UTF8.GetChars(Convert.FromBase64String(keeey))).Split(':');
            return new Credential(owner, unpass[0], unpass[1]);
        }
        /// <summary>
        /// Saves a credential to memory card
        /// </summary>
        /// <param name="Credentials"></param>
        public static void WriteToFile(Credential Credentials)
        {
            FileStream fs = new FileStream(@"\SD\" + "NsC" + ".crdn", FileMode.Create, FileAccess.Write);
            StreamWriter Writer = new StreamWriter(fs);
            Writer.WriteLine(Credentials.ServerOwner);
            Writer.WriteLine(Credentials.Key);
            Writer.Close();
            fs.Close();
        }
        /// <summary>
        /// Override of ToString() method
        /// </summary>
        /// <returns>Returns a string with credential parameters each followed by a new line</returns>
        public override string ToString()
        {
            return "Server Owner : " + ServerOwner + "\n" +
                "UserName : " + UserName + "\n" +
                "Password : " + Password + "\n" +
                "Encrypted User & Password : " + Key;
        }
    }
}

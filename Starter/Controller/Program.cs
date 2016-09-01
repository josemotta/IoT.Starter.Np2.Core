using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

using System.IO;
using HttpLibrary;
using HttpFileServer;

namespace Home
{
    public class Program
    {
        static HttpServer Server;                   // server object
        static Credential ServerCredential;         // server security
        static Configuration ServerConfiguration;   // configuration settings
        static bool status = false;                 // on board led status

        public static void Main()
        {
            TimeSpan elapsed = TimeSpan.Zero;

            // Try to get clock at system start
            try
            {
                var time = NtpClient.GetNetworkTime();
                Utility.SetLocalTime(time);
            }
            catch (Exception ex)
            {
                // Don't depend on time
                Debug.Print("Error setting clock: " + ex.Message);
            }

            // On board led
            OutputPort onBoardLed = new OutputPort(Pins.ONBOARD_LED, false);

            Thread.Sleep(1000);

            // Web Server
            ServerConfiguration = new Configuration(80);
            ServerCredential = new Credential("Administrator", "admin", "admin");
            Server = new HttpServer(ServerConfiguration, ServerCredential, @"\SD\");
            Server.OnServerError += new OnServerErrorDelegate(Server_OnServerError);
            Server.OnRequestReceived += new OnRequestReceivedDelegate(Server_OnRequestReceived);
            Server.Start();

            // File Server
            FileServer server = new FileServer(@"\SD\", 1554);

            while (true)
            {
                // null task

                onBoardLed.Write(status);
                Thread.Sleep(500);
                
            }
        }

        static void Server_OnRequestReceived(HttpRequest Request, HttpResponse Response)
        {
            if (Request.RequestedCommand != null)
            {
                switch (Request.RequestedCommand.ToLower())
                {
                    case "on":
                        status = true;      // on board led ON
                        break;
                    case "off":
                        status = false;     // on board led OFF
                        break;
                } 

                Response.WriteFilesList("<br>" + "Comando " + Request.RequestedCommand.ToLower() + ": Status = " + status.ToString());
            }
            else if (Request.RequestedFile != null)
            {
                string FullFileName = Request.FilesPath + Request.RequestedFile;
                if (File.Exists(FullFileName))
                {
                    Response.WriteFile(FullFileName);
                }
                else
                {
                    Response.WriteNotFound();
                }
            }
            else
            {
                Response.WriteFile(Request.FilesPath + "home.html"); // TODO: product page
            }
        }

        static void Server_OnServerError(ErrorEventArgs e)
        {
            Debug.Print(e.EventMessage);
        }

    }
}
using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using Microsoft.SPOT.Net.NetworkInformation;


namespace HttpLibrary
{
    /// <summary>
    /// Delegate for Recieve event handling
    /// </summary>
    /// <param name="Request"></param>
    /// <param name="Response"></param>
    public delegate void OnRequestReceivedDelegate(HttpRequest Request, HttpResponse Response);
    /// <summary>
    /// Delegate for error event handling
    /// </summary>
    /// <param name="e"></param>
    public delegate void OnServerErrorDelegate(ErrorEventArgs e);

    /// <summary>
    /// HttpServer class that handles Http requests and respoones
    /// </summary>
    public class HttpServer
    {
        private Thread server_thread;
        private Socket listen_socket;
        private Socket accepted_socket;
        private bool is_server_running;
        private string storage_path;
        private byte[] receive_buffer;
        private byte[] send_buffer;
        private bool is_polled;
        private int data_size;
        
        private Configuration server_config;
        private Credential server_credential;
        private bool use_authentication;
        private const string authentication_header = "HTTP/1.1 401 Authorization Required \nWWW-Authenticate: Basic realm=";
        private const string Unauthorized_page = "<html><body><h1 align=center>" + "401 UNAUTHORIZED ACCESS</h1></body></html>";     
        
        private void process_request()
        {
            is_polled = accepted_socket.Poll(50000, SelectMode.SelectRead);
            if (is_polled)
            {
                data_size = accepted_socket.Available;
                receive_buffer = new byte[data_size];
                if (data_size > 0)
                {
                    accepted_socket.Receive(receive_buffer, 0, receive_buffer.Length, SocketFlags.None);
                    if (use_authentication)
                    {
                        if (authenticate(receive_buffer))
                        {
                            OnRequestReceivedFunction(new HttpRequest(this.receive_buffer, this.storage_path, this.accepted_socket),
                                new HttpResponse(this.send_buffer, this.storage_path, this.accepted_socket));
                        }
                        else
                        {
                            byte[] header = UTF8Encoding.UTF8.GetBytes(authentication_header + server_credential.ServerOwner + "\"\n\n");
                            accepted_socket.Send(header, 0, header.Length, SocketFlags.None);
                            accepted_socket.Send(UTF8Encoding.UTF8.GetBytes(Unauthorized_page), 0, Unauthorized_page.Length, SocketFlags.None);
                        }
                    }
                    else
                    {
                        OnRequestReceivedFunction(new HttpRequest(this.receive_buffer, this.storage_path, this.accepted_socket),
                                new HttpResponse(this.send_buffer, this.storage_path, this.accepted_socket));
                    }
                }
                receive_buffer = null;
            }
        }
        private void run_server()
        {
            try
            {
                listen_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint BindingAddress = new IPEndPoint(IPAddress.Any, server_config.ListenPort);
                listen_socket.Bind(BindingAddress);
                listen_socket.Listen(1);
                is_server_running = true;
                while (true)
                {
                    accepted_socket = listen_socket.Accept();
                    process_request();
                    accepted_socket.Close();
                }
            }
            catch (Exception)
            {
                is_server_running = false;
                OnServerErrorFunction(new ErrorEventArgs("Server Error\r\nCheck Connection Parameters"));
            }
        }
        private bool authenticate(byte[] request)
        {
            string reqstr=new string(UTF8Encoding.UTF8.GetChars(request));
            return (reqstr.IndexOf(server_credential.Key) >= 0);
        }

        /// <summary>
        /// Used in event firing
        /// </summary>
        /// <param name="Error">Parameter</param>
        protected virtual void OnServerErrorFunction(ErrorEventArgs Error)
        {
            OnServerError(Error);
        }

        /// <summary>
        /// Used in event firing
        /// </summary>
        /// <param name="Request">Parameter</param>
        /// <param name="Response">Parameter</param>
        protected virtual void OnRequestReceivedFunction(HttpRequest Request, HttpResponse Response)
        {
            OnRequestReceived(Request, Response);
        }

        /// <summary>
        /// Gets or sets the send buffer size
        /// </summary>
        public int BufferSize
        {
            set { send_buffer = null; send_buffer = new byte[value]; }
            get { return this.send_buffer.Length; }
        }

        /// <summary>
        /// Gets if credentials are enabled
        /// </summary>
        public bool SecurityEnabled
        {
            get
            {
                return this.use_authentication;
            }
        }
        /// <summary>
        /// Gets the server configuration
        /// </summary>
        public Configuration Settings
        {
            get
            {
                return this.server_config;
            }
        }
        /// <summary>
        /// Gets if server is running
        /// </summary>
        public bool IsServerRunning
        {
            get { return this.is_server_running; }
        }
        /// <summary>
        /// Gets the current server running thread
        /// </summary>
        public Thread RunningThread
        {
            get { return this.server_thread; }
        }
        /// <summary>
        /// Gets the server credentials
        /// </summary>
        public Credential Security
        {
            get { return this.server_credential; }
        }

        /// <summary>
        /// ServerError event
        /// </summary>
        public event OnServerErrorDelegate OnServerError;
        /// <summary>
        /// RequestRecieved event
        /// </summary>
        public event OnRequestReceivedDelegate OnRequestReceived;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="Config">Server configuration</param>
        /// <param name="PagesDirectory">Location where pages are stored</param>
        public HttpServer(Configuration Config, string PagesDirectory)
        {
            this.server_thread = null;
            this.listen_socket = null;
            this.accepted_socket = null;
            this.is_server_running = false;
            this.storage_path = PagesDirectory;
            this.receive_buffer = null;
            this.send_buffer = new byte[256];
            is_polled = false;
            data_size = 0;
            this.server_thread = new Thread(new ThreadStart(run_server));
            this.server_config = Config;
            this.use_authentication = false;
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];
            networkInterface.EnableStaticIP(server_config.IpAddress, server_config.SubnetMask, server_config.DefaultGateWay);
            Thread.Sleep(1000);
        }
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="Config">Server configuration</param>
        /// <param name="Security">Server credentials</param>
        /// <param name="PagesDirectory">Location where pages are stored</param>
        public HttpServer(Configuration Config, Credential Security, string PagesDirectory)
        {
            this.server_thread = null;
            this.listen_socket = null;
            this.accepted_socket = null;
            this.is_server_running = false;
            this.storage_path = PagesDirectory;
            this.receive_buffer = null;
            this.send_buffer = new byte[256];
            is_polled = false;
            data_size = 0;
            this.server_thread = new Thread(new ThreadStart(run_server));
            this.server_config = Config;
            this.server_credential = Security;
            this.use_authentication = true;
         
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];
            networkInterface.EnableStaticIP(server_config.IpAddress, server_config.SubnetMask, server_config.DefaultGateWay);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Starts the server listener 
        /// </summary>
        public void Start()
        {
            this.server_thread.Start();
        }
        /// <summary>
        /// Stops the server listener
        /// </summary>
        public void Stop()
        {
            this.listen_socket.Close();
        }        
    }
}

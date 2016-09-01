using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace HttpLibrary
{
    /// <summary>
    /// HttpRequest class for handeling http requests
    /// </summary>
    public class HttpRequest
    {
        private string request_buffer;
        private string files_path;
        private string http_method;
        private int request_length;
        private string host_address;
        private Socket connection;
        /// <summary>
        /// Gets the http request entire string
        /// </summary>
        public string QueryString { get { return this.request_buffer; } }
        /// <summary>
        /// Gets where server files are located
        /// </summary>
        public string FilesPath { get { return this.files_path; } }
        /// <summary>
        /// Gets the http method GET or POST or null
        /// </summary>
        public string HttpMethod { get { return this.http_method; } }
        /// <summary>
        /// Gets the request length
        /// </summary>
        public int Length { get { return this.request_length; } }
        /// <summary>
        /// Gets the remote host address
        /// </summary>
        public string HostAddress { get { return this.host_address; } }
        /// <summary>
        /// Gets the current active socket
        /// </summary>
        public Socket Connection { get { return this.connection; } }
        /// <summary>
        /// Gets the requested page or file
        /// </summary>
        public string RequestedFile
        {
            get
            {
                int Count;
                int IndexOfSlash = request_buffer.IndexOf('/') + 1;
                for (Count = IndexOfSlash; request_buffer[Count] != ' ' && request_buffer[Count] != '?'; Count++) ;
                int Length = Count - IndexOfSlash;
                string Name=this.request_buffer.Substring(IndexOfSlash, Length);
                return (Name == "") ? null : Name;
            }
        }
        /// <summary>
        /// Gets the requested IR command
        /// </summary>
        public string RequestedCommand
        {
            get
            {
                int Count;
                int Index = request_buffer.IndexOf('/') + 1;
                for (Count = Index; request_buffer[Count] != ' ' && request_buffer[Count] != '?'; Count++) ;
                int Length = Count - Index;
                string Prefix = this.request_buffer.Substring(Index, Length);
                if (Prefix != "cmd" && Prefix != "CMD") return null;
                for (Index = ++Count; request_buffer[Count] != ' ' && request_buffer[Count] != '?'; Count++) ;
                Length = Count - Index;
                string Command = this.request_buffer.Substring(Index, Length);
                return (Command == "") ? null : Command;
            }
        }
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="buffer">Recieved buffer</param>
        /// <param name="FilesPath">Server files Location</param>
        /// <param name="Connection">Current active socket</param>
        public HttpRequest(byte[] buffer, string FilesPath, Socket Connection)
        {
            this.request_buffer = new string(UTF8Encoding.UTF8.GetChars(buffer));
            this.files_path = FilesPath;
            if (this.request_buffer.IndexOf("GET") >= 0)
                this.http_method = "GET";
            else if (this.request_buffer.IndexOf("POST") >= 0)
                this.http_method = "POST";
            else
                throw new Exception("Only GET And POST Methods Are Supported");
            this.request_length = this.request_buffer.Length;
            this.host_address = ((IPEndPoint)Connection.RemoteEndPoint).Address.ToString();
            this.connection = Connection;
        }
        /// <summary>
        /// Indexer to find values submitted by a form html
        /// </summary>
        /// <param name="value">Value name</param>
        /// <returns></returns>
        public string this[string value]
        {
            get
            {
                int IndexOfValue = request_buffer.IndexOf(value + "=");
                int i;
                StringBuilder sb = new StringBuilder();
                if (IndexOfValue >= 0)
                {
                    for (i = IndexOfValue; request_buffer[i] != '='; ++i) ;
                    for (++i; request_buffer[i] != '&' && request_buffer[i] != ' '; i++)
                        sb.Append(request_buffer[i]);
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

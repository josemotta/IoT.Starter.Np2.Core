using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.SPOT.Net.NetworkInformation;

namespace HttpFileServer
{
    public class FileServer : IDisposable
    {
        private const int Backlog = 1;
        private Socket _socket = null;
        private Thread _thread = null;
        private string _location = null;

        public FileServer(string location, int port = 80)
        {
            _location = location;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(Backlog);

            NetworkInterface[] ips = NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < ips.Length; i++)
            {
                Debug.Print(ips[0].IPAddress.ToString() + ": " + port);
            }

            _thread = new Thread(new ThreadStart(ListenForClients));
            _thread.Start();
        }

//        private const int BufferSize = 2048;
        private const int BufferSize = 500;

        private void ListenForClients()
        {
            while (true)
            {
                using (Socket client = _socket.Accept())
                {
                    // Wait for data to become available
                    while (!client.Poll(10, SelectMode.SelectRead)) { }

                    if (client.Available > 0)
                    {
                        byte[] buffer = new byte[sizeof(int)];
                        client.Receive(buffer, sizeof(int), SocketFlags.None);

                        Operation op = (Operation)BitConverter.ToInt32(buffer, 0);

                        client.Receive(buffer, sizeof(int), SocketFlags.None);
                        int length = (int)BitConverter.ToInt32(buffer, 0);

                        if (length > BufferSize)
                        {
                            int received = 0;
                            while (received < length)
                            {
                                int xferSize = System.Math.Min(BufferSize, length - received);
                                buffer = new byte[xferSize];
                                client.Receive(buffer, 0, xferSize, SocketFlags.None);
                                received += xferSize;
                            }

                            SendFailure(client);
                        }
                        else if (length == 0)
                        {
                            Process(client, op, "");
                        }
                        else
                        {
                            buffer = new byte[length];
                            client.Receive(buffer, 0, length, SocketFlags.None);
                            string file = new string(Encoding.UTF8.GetChars(buffer));
                            Process(client, op, file);
                        }
                    }
                }
            }
        }

        private void SendFailure(Socket client)
        {
            byte[] retCode = BitConverter.GetBytes((int)ReturnCode.Failure);
            client.Send(retCode, retCode.Length, SocketFlags.None);

            // Workaround:
            // http://forums.netduino.com/index.php?/topic/4555-socket-error-10055-wsaenobufs/
            Thread.Sleep(100);
        }

        private void SendSuccess(Socket client)
        {
            byte[] retCode = BitConverter.GetBytes((int)ReturnCode.Success);
            client.Send(retCode, retCode.Length, SocketFlags.None);

            // Workaround:
            // http://forums.netduino.com/index.php?/topic/4555-socket-error-10055-wsaenobufs/
            Thread.Sleep(100);
        }

        private void Process(Socket client, Operation op, string file)
        {
            switch (op)
            {
                case Operation.List:
                    ListDirectory(client, file);
                    break;
                case Operation.Get:
                    SendFile(client, file);
                    break;
                case Operation.Put:
                    ReceiveFile(client, file);
                    break;
                case Operation.Delete:
                    DeleteFile(client, file);
                    break;
                default:
                    SendFailure(client);
                    break;
            }
        }

        private void DeleteFile(Socket client, string file)
        {
            string fullPath = _location + @"\" + file;
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                SendSuccess(client);
            }
            else if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                SendSuccess(client);
            }
            else
            {
                SendFailure(client);
            }
        }

        private void ReceiveFile(Socket client, string file)
        {
            using (FileStream stream = File.Open(_location + @"\" + file, FileMode.Create))
            {
                SendSuccess(client);

                byte[] buffer = new byte[sizeof(int)];
                client.Receive(buffer, sizeof(int), SocketFlags.None);
                int length = (int)BitConverter.ToInt32(buffer, 0);

                int received = 0;
                while (received < length)
                {
                    int remSize = (int)(length - received);
                    int xferSize = System.Math.Min(remSize, BufferSize);
                    buffer = new byte[xferSize];
                    client.Receive(buffer, 0, xferSize, SocketFlags.None);
                    stream.Write(buffer, 0, xferSize);
                    received += xferSize;

                    // Workaround:
                    // http://forums.netduino.com/index.php?/topic/4555-socket-error-10055-wsaenobufs/
                    Thread.Sleep(100);
                }

                SendSuccess(client);
            }
        }

        private void SendFile(Socket client, string file)
        {
            string filename = _location + @"\" + file;
            if (File.Exists(filename))
            {
                SendSuccess(client);

                using (FileStream stream = File.Open(filename, FileMode.Open))
                {
                    int length = (int)stream.Length;
                    byte[] size = BitConverter.GetBytes(length);
                    client.Send(size, size.Length, SocketFlags.None);

                    // Workaround:
                    // http://forums.netduino.com/index.php?/topic/4555-socket-error-10055-wsaenobufs/
                    Thread.Sleep(100);

                    while (stream.Position < stream.Length)
                    {
                        int remSize = (int)(stream.Length - stream.Position);
                        int xferSize = System.Math.Min(remSize, BufferSize);
                        byte[] buffer = new byte[xferSize];
                        stream.Read(buffer, 0, xferSize);
                        client.Send(buffer, 0, xferSize, SocketFlags.None);

                        // Workaround:
                        // http://forums.netduino.com/index.php?/topic/4555-socket-error-10055-wsaenobufs/
                        Thread.Sleep(100);
                    }

                    SendSuccess(client);
                }
            }
            else
            {
                SendFailure(client);
            }
        }

        private void ListDirectory(Socket client, string dir)
        {
            string[] files = Directory.GetFiles(_location + @"\" + dir);

            SendSuccess(client);
            byte[] numFiles = BitConverter.GetBytes(files.Length);
            client.Send(numFiles, numFiles.Length, SocketFlags.None);
            SendSuccess(client);

            foreach (string file in files)
            {
                string filename = file.Substring(file.LastIndexOf('\\') + 1);
                byte[] data = Encoding.UTF8.GetBytes(filename);

                byte[] fileSize = BitConverter.GetBytes(data.Length);
                client.Send(fileSize, fileSize.Length, SocketFlags.None);

                // Workaround:
                // http://forums.netduino.com/index.php?/topic/4555-socket-error-10055-wsaenobufs/
                Thread.Sleep(100);

                client.Send(data, data.Length, SocketFlags.None);

                // Workaround:
                // http://forums.netduino.com/index.php?/topic/4555-socket-error-10055-wsaenobufs/
                Thread.Sleep(100);

                SendSuccess(client);
            }
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _socket.Close();
                    _thread.Abort();
                }

                _disposed = true;

            }
        }

        ~FileServer()
        {
            Dispose(false);
        }
        #endregion
    }
}

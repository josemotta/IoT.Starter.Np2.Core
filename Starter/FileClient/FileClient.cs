using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace FileClient
{
    public enum Operation
    {
        List,
        Get,
        Put,
        Delete
    }

    public enum ReturnCode
    {
        Success,
        Failure
    }

    public static class Client
    {
        private static IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.104"), 1554);

        private static ReturnCode GetStatus(Socket socket)
        {
            byte[] status = new byte[sizeof(int)];
            socket.Receive(status, status.Length, SocketFlags.None);
            return (ReturnCode)BitConverter.ToInt32(status, 0);
        }

        public static bool Send(string file)
        {
            if(!File.Exists(file))
            {
                return false;
            }

            using(Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(ipEndPoint);

                socket.Send(BitConverter.GetBytes((int)Operation.Put));
                byte[] filename = Encoding.UTF8.GetBytes(file.Substring(file.LastIndexOf('\\') + 1));
                socket.Send(BitConverter.GetBytes(filename.Length));
                socket.Send(filename);

                ReturnCode retCode = GetStatus(socket);

                if(retCode == ReturnCode.Failure)
                {
                    return false;
                }

                using(FileStream stream = File.Open(file, FileMode.Open))
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    socket.Send(BitConverter.GetBytes((int)data.Length));
                    socket.Send(data);

                    return GetStatus(socket) == ReturnCode.Success;
                }
            }
        }

        public static bool Delete(string file)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(ipEndPoint);

                socket.Send(BitConverter.GetBytes((int)Operation.Delete));
                byte[] filename = Encoding.UTF8.GetBytes(file);
                socket.Send(BitConverter.GetBytes(filename.Length));
                socket.Send(filename);

                return GetStatus(socket) == ReturnCode.Success;
            }
        }

        public static bool Receive(string remoteFile, string localFile)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(ipEndPoint);

                socket.Send(BitConverter.GetBytes((int)Operation.Get));
                byte[] filename = Encoding.UTF8.GetBytes(remoteFile);
                socket.Send(BitConverter.GetBytes(filename.Length));
                socket.Send(filename);

                ReturnCode retCode = GetStatus(socket);

                if (retCode == ReturnCode.Failure)
                {
                    return false;
                }

                using (FileStream stream = File.Open(localFile, FileMode.Create))
                {
                    byte[] size = new byte[sizeof(int)];
                    socket.Receive(size);
                    int length = BitConverter.ToInt32(size, 0);

                    byte[] data = new byte[length];
                    int total = 0;
                    while (total < length)
                    {
                        total += socket.Receive(data, total, length - total, SocketFlags.None);
                    }

                    stream.Write(data, 0, data.Length);

                    return GetStatus(socket) == ReturnCode.Success;
                }
            }
        }

        public static bool List(string dir, out string[] files)
        {
            files = null;

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(ipEndPoint);

                socket.Send(BitConverter.GetBytes((int)Operation.List));
                byte[] filename = Encoding.UTF8.GetBytes(dir);
                socket.Send(BitConverter.GetBytes(filename.Length));
                socket.Send(filename);

                ReturnCode retCode = GetStatus(socket);

                if (retCode == ReturnCode.Failure)
                {
                    return false;
                }

                byte[] size = new byte[sizeof(int)];
                socket.Receive(size);
                int numFiles = BitConverter.ToInt32(size, 0);

                files = new string[numFiles];

                retCode = GetStatus(socket);

                if (retCode == ReturnCode.Failure)
                {
                    return false;
                }

                for (int i = 0; i < numFiles; i++)
                {
                    socket.Receive(size);
                    int fileSize = BitConverter.ToInt32(size, 0);

                    byte[] data = new byte[fileSize];
                    socket.Receive(data);

                    files[i] = Encoding.UTF8.GetString(data);

                    retCode = GetStatus(socket);

                    if (retCode == ReturnCode.Failure)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}

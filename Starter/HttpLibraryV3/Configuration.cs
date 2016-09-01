using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Microsoft.SPOT.Net.NetworkInformation;


namespace HttpLibrary
{
    /// <summary>
    /// Configuration class for holding server configuration
    /// </summary>
    public class Configuration
    {
        private bool DHCP;

        /// <summary>
        /// Gets if Dhcp is Enabled
        /// </summary>
        public bool DhcpEnabled
        {
            get { return this.DHCP; }
        }
        /// <summary>
        /// Listening ip address
        /// </summary>
        public string IpAddress;
        /// <summary>
        /// Network mask
        /// </summary>
        public string SubnetMask;
        /// <summary>
        /// Network default gateway
        /// </summary>
        public string DefaultGateWay;
        /// <summary>
        /// Listening port
        /// </summary>
        public int ListenPort;
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="IpAddress">Listening ip address</param>
        /// <param name="SubnetMask">Network mask</param>
        /// <param name="DefaultGateWay">Default gateway</param>
        /// <param name="ListenPort">Listening port</param>
        public Configuration(string IpAddress, string SubnetMask, string DefaultGateWay, int ListenPort)
        {
            this.DHCP = false;
            this.IpAddress = IpAddress;
            this.SubnetMask = SubnetMask;
            this.DefaultGateWay = DefaultGateWay;
            this.ListenPort = ListenPort;
        }
        /// <summary>
        /// Class constructor, initializes configuration as DHCP
        /// </summary>
        /// <param name="ListenPort">Listening port</param>
        public Configuration(int ListenPort)
        {
            this.DHCP = true;
            NetworkInterface Interface = NetworkInterface.GetAllNetworkInterfaces()[0];
            Interface.EnableDhcp();
            while (true)
            {
                Interface = NetworkInterface.GetAllNetworkInterfaces()[0];
                if (Interface.IPAddress != "0.0.0.0")
                {
                    this.IpAddress = Interface.IPAddress;
                    this.SubnetMask = Interface.SubnetMask;
                    this.DefaultGateWay = Interface.GatewayAddress;
                    this.ListenPort = ListenPort;
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Override of ToString() method
        /// </summary>
        /// <returns>A string with configuration parameters each followed by a new line</returns>
        public override string ToString()
        {
            return "IpAddress : " + IpAddress + "\n" +
                "SubnetMask : " + SubnetMask + "\n" +
                "Default GateWay : " + DefaultGateWay + "\n" +
                "Port : " + ListenPort.ToString();
        }
    }
}

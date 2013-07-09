using System;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;


namespace rarp
{
    class Rarp
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true, EntryPoint = "SendARP")]
        public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);


        public static string GetIP(string mac)
        {
            byte[] pMac = StringToBytesMAC(mac);
            byte[] tempMac = new byte[6];
            int len = 6;

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
                foreach (UnicastIPAddressInformation ipInfo in adapter.GetIPProperties().UnicastAddresses)
                    if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        foreach (int ip in GetNeighbourIP(ipInfo.Address, ipInfo.IPv4Mask))
                        {
                            if (SendARP(ip, 0, tempMac, ref len) == 0
                                && compareMac(pMac, tempMac) == true)
                                return new IPAddress(ip).ToString();
                        }
                    }

            return null;
        }

        private static byte[] StringToBytesMAC(string mac)
        {
            string[] b = new string[6];
            byte[] res = new byte[6];
            mac = string.Join("", mac.Split('-'));
            mac = string.Join("", mac.Split(':'));
            mac = string.Join("", mac.Split('.'));
            if (mac.Length != 12)
                throw new FormatException("MAC address should be delimited by ':' or '-'.\n" +
                                          "Alternatively it should have exactly 12 hexadecimal characters.");
            else
                for (int i = 0; i < 12; i += 2)
                    b[i / 2] = mac.Substring(i, 2);
            for (int i = 0; i < 6; i++)
                res[i] = Convert.ToByte(b[i], 16);
            return res;
        }

        private static IEnumerable<int> GetNeighbourIP(IPAddress hostIP, IPAddress netmask)
        {
            byte[] ipBytes = hostIP.GetAddressBytes();
            byte[] maskBytes = netmask.GetAddressBytes();
            uint iIP = (uint) IPAddress.HostToNetworkOrder((int)hostIP.Address);
            uint iMask = (uint) IPAddress.HostToNetworkOrder((int)netmask.Address);

            if (ipBytes.Length != maskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] netBytes = new byte[ipBytes.Length];
            for (int i = 0; i < netBytes.Length; i++)
                netBytes[i] = (byte)(ipBytes[i] & (maskBytes[i]));
            Array.Reverse(netBytes);
            uint iNet = BitConverter.ToUInt32(netBytes, 0);

            for (uint tempIP = iNet + 1; tempIP < iNet + ~iMask; tempIP++)
                if (tempIP != iIP)
                    yield return IPAddress.HostToNetworkOrder((int) tempIP);
        }

        private static bool compareMac(byte[] first, byte[] second)
        {
            if (first.Length != second.Length)
                return false;
            for (int i = 0; i < first.Length; i++)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: rarp.exe mac-address");
                return;
            }
            System.Console.WriteLine(Rarp.GetIP(args[0]));
        }
    }
}

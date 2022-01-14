using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using GameModelsLib;
using System.IO;

namespace GameModelsLib
{
    public static class UDPClient
    {

        public static bool alive = false; 
        public static UdpClient client;
        public static int LOCALPORT = 8002;
        public static int REMOTEPORT = 8001; 
        public static int TTL = 20;
        public static string HOST = "235.5.5.1"; 
        public static IPAddress groupAddress; 

        public static string userName; 

        public static void ReadPortsSettings()
        {

            if (File.Exists(userName + ".txt"))
            {
                StreamReader sr = new StreamReader(userName + ".txt");

                LOCALPORT = Convert.ToInt32(sr.ReadLine());
                REMOTEPORT = Convert.ToInt32(sr.ReadLine());

                sr.Close();

                groupAddress = IPAddress.Parse(HOST);

                client = new UdpClient(LOCALPORT);
              
                client.JoinMulticastGroup(groupAddress, TTL);

            }
            else
            {
                MessageBox.Show("File not found");
            }
        }

        public static void SendMessageInfo(string info)
        {
            try
            {
                string message = info;
                byte[] data = Encoding.Unicode.GetBytes(message);
                UDPClient.client.Send(data, data.Length, UDPClient.HOST, UDPClient.REMOTEPORT);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}

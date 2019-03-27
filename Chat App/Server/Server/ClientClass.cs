using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Server
{
    public class ClientClass
    {
        TcpClient tcpClient;
        public NetworkStream networkStream;
        public StreamWriter streamWriter;
        public StreamReader streamReader;
        public string clientname;
        public ClientClass(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            networkStream = tcpClient.GetStream();
        }

        public void send(byte[] str)
        {
            networkStream.Write(str, 0, str.Length);
        }


    }
}

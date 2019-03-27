using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TcpListener tcpListener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 8001));
        List<ClientClass> clientlist = new List<ClientClass>();
        private void button2_Click(object sender, EventArgs e)
        {
            tcpListener.Start();
            //Accept
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(acceptclients), tcpListener);
        }

        //Accept
        private void acceptclients(IAsyncResult asyncResult)
        {

            tcpListener = (TcpListener)asyncResult.AsyncState;
            TcpClient tcpClient = tcpListener.EndAcceptTcpClient(asyncResult);
            ClientClass clientClass = new ClientClass(tcpClient);
            byte[] arr = new byte[1024];
            int c = clientClass.networkStream.Read(arr, 0, arr.Length);
            clientClass.clientname = ASCIIEncoding.ASCII.GetString(arr, 0, c);
            this.Invoke(new Action(() => 
            {
                comboBox1.Items.Add(clientClass.clientname);
                MessageBox.Show(clientClass.clientname+" : Connected");
            }));

            clientlist.Add(clientClass);
            //Read
            clientClass.networkStream.BeginRead(arr, 0, arr.Length, new AsyncCallback(Read), new object[] { clientClass, arr });
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(acceptclients), tcpListener);
        }

        //Read
        //Decrypted
        private void Read(IAsyncResult ar)
        {
            CryptoStream cs;
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes("12345678");
            des.IV = ASCIIEncoding.ASCII.GetBytes("12345678");

            object[] obj = (object[])ar.AsyncState;
            ClientClass clientClass = (ClientClass)obj[0];
            byte[] arr = (byte[])obj[1];
            int count = clientClass.networkStream.EndRead(ar);

            cs = new CryptoStream(ms, des.CreateDecryptor(),
                    CryptoStreamMode.Write);
            cs.Write(arr, 0, count);
            cs.FlushFinalBlock();
            this.Invoke(new Action(() => 
            {
                string msg = ASCIIEncoding.ASCII.GetString(ms.ToArray());
                listBox1.Items.Add(clientClass.clientname+" : "+msg);
                ms.Flush();
                // listBox1.Items.Add(clientClass.clientname + " : " + Encoding.ASCII.GetString(arr, 0, count));
            }));
            clientClass.networkStream.Flush();
            clientClass.networkStream.BeginRead(arr, 0, arr.Length, new AsyncCallback(Read), new object[] { clientClass, arr });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string check = comboBox1.Text;
            byte[] arr = new byte[1024];
            arr = ASCIIEncoding.ASCII.GetBytes(textBox1.Text);

            if (check=="All")
            {

                foreach (ClientClass item in clientlist)
                {

                    item.networkStream.Write(arr, 0, arr.Length);
                }
            }
            else
            {
                ClientClass client = clientlist.Find(p => p.clientname == check);
                client.networkStream.Write(arr, 0, arr.Length);
            }

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

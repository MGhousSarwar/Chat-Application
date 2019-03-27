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

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpClient tcpClient = new TcpClient();
        NetworkStream networkStream = null;
        StreamWriter streamWriter;
        StreamReader streamReader;
        string clientname;
        private void button2_Click(object sender, EventArgs e)
        {
            tcpClient.Connect(new IPEndPoint(IPAddress.Parse("192.168.8.102"), 8001));
          networkStream= tcpClient.GetStream();

            byte[] arr = new byte[1024];
            clientname = textBox2.Text;
            arr = ASCIIEncoding.ASCII.GetBytes(clientname);
            networkStream.Write(arr, 0, arr.Length);
            arr = new byte[1024];
            networkStream.BeginRead(arr, 0, arr.Length, new AsyncCallback(Read), new object[] { networkStream, arr });
        }

        private void Read(IAsyncResult ar)
        {
            object[] obj = (object[])ar.AsyncState;
            NetworkStream ns = (NetworkStream)obj[0];
            byte[] arr = (byte[])obj[1];
            int count = ns.EndRead(ar);
            this.Invoke(new Action(() =>
            {
                listBox1.Items.Add("Server : " + Encoding.ASCII.GetString(arr, 0, count));
            }));
            ns.Flush();
            ns.BeginRead(arr, 0, arr.Length, new AsyncCallback(Read), new object[] { ns, arr });
        }

        //Encrypted
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(clientname + " To Sever : " +
              textBox1.Text);
            byte[] arr = new byte[1024];
            CryptoStream cs;
            MemoryStream ms = new MemoryStream();

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes("12345678");
            des.IV = ASCIIEncoding.ASCII.GetBytes("12345678");

            cs = new CryptoStream(ms, des.CreateEncryptor(),
                CryptoStreamMode.Write);
            arr = ASCIIEncoding.ASCII.GetBytes(textBox1.Text);
            cs.Write(arr, 0, arr.Length);
            cs.FlushFinalBlock();
            networkStream.Write(ms.ToArray(), 0, ms.ToArray().Length);
            ms.Flush();

            //arr = ASCIIEncoding.ASCII.GetBytes(textBox1.Text);
            //networkStream.Write(arr, 0, arr.Length);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //streamWriter.WriteLine("Discard Request !@#$%^");
            //networkStream.Close();
            //tcpClient.Close();
            Environment.Exit(0);

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

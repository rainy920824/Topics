using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;

namespace go2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            label3.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(textBox1.Text, int.Parse(textBox2.Text));
            NetworkStream ns = tcpClient.GetStream();
            BinaryWriter writer = new BinaryWriter(ns);
            FileStream fileStream = new FileStream(label3.Text, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[524288];                                                             //1KB
            int len;
            while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                // 傳送分片大小
                writer.Write(len);

                // 傳送分片內容
                writer.Write(buffer, 0, len);

            }
            writer.Write(0);
            label4.Text="檔案傳送完成。";
            //FileStream fin = new FileStream(label3.Text, FileMode.Open);
            //byte[] buffer = new byte[1024];
            //int len;
            //len = fin.Read(buffer,0,buffer.Length);
            //while (len > 0)
            //{
            //    send_msg_to("size@" + len, textBox1.Text, int.Parse(textBox2.Text));
            //    len = fin.Read(buffer, 0, buffer.Length);
            //}
        }
        private void send_msg_to(string mydata, string ip, int port)
        {
            byte[] data = Encoding.UTF8.GetBytes(mydata);
            UdpClient udp = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            udp.Send(data, data.Length, ep);
        }
    }
}

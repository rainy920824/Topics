﻿using System;
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
            button2.Visible = true;
        }
        RSACryptoServiceProvider rsa1 = new RSACryptoServiceProvider();
        private void button2_Click(object sender, EventArgs e)
        {
            rsa1.FromXmlString(key);
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(textBox1.Text, int.Parse(textBox2.Text));
            NetworkStream networkStream = tcpClient.GetStream();
            BinaryWriter writer = new BinaryWriter(networkStream);
            FileStream fileStream = new FileStream(label3.Text, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[117];                                                             //1KB
            int len;
            while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                byte[] cipher = rsa1.Encrypt(buffer, false);
                // 傳送分片大小
                writer.Write(cipher.Length);

                // 傳送分片內容
                writer.Write(cipher, 0, cipher.Length);

            }
            writer.Write(0);
            label4.Text="檔案傳送完成。";
        }
        private void send_msg_to(string mydata, string ip, int port)
        {
            byte[] data = Encoding.UTF8.GetBytes(mydata);
            UdpClient udp = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            udp.Send(data, data.Length, ep);
        }
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 3001);
        string key;
        private void Form1_Load(object sender, EventArgs e)
        {
            tcpListener.Start();
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            NetworkStream networkStream = tcpClient.GetStream();
            BinaryReader reader = new BinaryReader(networkStream);
            key = reader.ReadString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
}

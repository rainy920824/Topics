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

namespace Output
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 3000);
        string savePath;
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            tcpListener.Start();
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            saveFileDialog1.ShowDialog();
            savePath = saveFileDialog1.FileName;
            NetworkStream networkStream = tcpClient.GetStream();
            BinaryReader reader = new BinaryReader(networkStream);
            FileStream fout = new FileStream(savePath, FileMode.Create);
            while (true)
            {
                // 分片大小
                int bytesRead = reader.ReadInt32();

                // 讀取=0，表示檔案接收完成
                if (bytesRead == 0)
                {
                    label1.Text = "檔案接收完成。";
                    break;
                }

                // 分片內容
                byte[] buffer = reader.ReadBytes(bytesRead);
                byte[] plain1 = rsa.Decrypt(buffer, false);
                fout.Write(plain1, 0, plain1.Length);
            }
            tcpListener.Stop();
            fout.Close();
            button1.Visible = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(textBox1.Text, int.Parse(textBox2.Text));
            NetworkStream networkStream = tcpClient.GetStream();
            BinaryWriter writer = new BinaryWriter(networkStream);
            writer.Write(rsa.ToXmlString(false));
            button1_Click(sender, e);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

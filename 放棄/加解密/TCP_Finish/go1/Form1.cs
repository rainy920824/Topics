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
using System.IO;
namespace go1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //int localPort = 12345;
            //TcpListener tcpListener = new TcpListener(IPAddress.Any, localPort);
            //tcpListener.Start();
            //TcpClient tcpClient = tcpListener.AcceptTcpClient();
            //NetworkStream networkStream = tcpClient.GetStream();
            //BinaryReader reader = new BinaryReader(networkStream);
            //saveFileDialog1.ShowDialog();
            //string savePath = saveFileDialog1.FileName;
            //FileStream fileStream = new FileStream(savePath, FileMode.Append, FileAccess.Write);
            //while (true)
            //{
            //    // 接收分片大小
            //    int bytesRead = reader.ReadInt32();

            //    // 如果讀取到的大小為0，表示檔案接收完成
            //    if (bytesRead == 0)
            //    {
            //        label1.Text = "檔案接收完成。";
            //        break;
            //    }

            //    // 接收分片內容
            //    byte[] buffer = reader.ReadBytes(bytesRead);
            //    fileStream.Write(buffer, 0, bytesRead);
            //}
            //tcpListener.Stop();
            //byte[] rawdata = udp.Receive(ref ep);
            //byte[] buffer = new byte[1024];
            //string rawstring = Encoding.UTF8.GetString(rawdata);
            //string[] token = rawstring.Split('@');
            //switch (token[0])
            //{
            //    case "size":
            //        op1 = token[1];
            //        fout.Write(buffer, 0, int.Parse(op1));
            //        break;
            //}
        }
        string savePath;
        private void Form1_Load(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            savePath = saveFileDialog1.FileName;
            button1_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int localPort = 12345;
            TcpListener tcpListener = new TcpListener(IPAddress.Any, localPort);
            tcpListener.Start();
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            NetworkStream networkStream = tcpClient.GetStream();
            BinaryReader reader = new BinaryReader(networkStream);

            FileStream fout = new FileStream(savePath, FileMode.Create);
            while (true)
            {
                // 分片大小
                int bytesRead = reader.ReadInt32();
                label1.Text = "傳輸中....";
                // 讀取=0，表示檔案接收完成
                if (bytesRead == 0)
                {
                    label1.Text = "檔案接收完成。";
                    break;
                }

                // 分片內容
                byte[] buffer = reader.ReadBytes(bytesRead);
                fout.Write(buffer, 0, bytesRead);
            }
            tcpListener.Stop();
            fout.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            savePath = saveFileDialog1.FileName;
            button1_Click(sender, e);
        }
    }
}

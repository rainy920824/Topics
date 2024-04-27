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
            try
            {
                label1.Text = "";
                tcpListener.Start();
                label1.Text = "傳輸中....";
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                saveFileDialog1.Filter = "PPT檔(*.pptx)|*.pptx|PDF檔(*.pdf) | *.pdf |Word檔(*.docx) | *.docx |Excel檔(*.xlsx) | *.xlsx |RAR檔(*.rar) | *.rar |文字檔(*.txt) | " + ""
                                       + "*.txt |JPG檔(*.jpg) | *.jpg |PNG檔(*.png) | *.png |影音檔(*.wav) | *.wav |影片檔(*.mp4) | *.mp4 ";
                saveFileDialog1.ShowDialog();
                savePath = saveFileDialog1.FileName;
                NetworkStream networkStream = tcpClient.GetStream();
                BinaryReader reader = new BinaryReader(networkStream);
                FileStream fout = new FileStream(savePath, FileMode.Create, FileAccess.Write);
                while (true)
                {
                    // 分片大小
                    int bytesss = reader.ReadInt32();

                    // 讀取=0，表示檔案接收完成
                    if (bytesss == 0)
                    {
                        label1.Text = "檔案接收完成。";
                        break;
                    }
                    // 分片內容
                    int bytesRead = reader.ReadInt32();
                    byte[] buffer = reader.ReadBytes(bytesRead);
                    byte[] plain1 = rsa.Decrypt(buffer, false);
                    if (bytesss == bytesRead)
                        fout.Write(plain1, 0, bytesRead);
                    else
                        fout.Write(plain1, 0, bytesss);
                }
                tcpListener.Stop();
                fout.Close();
                button1.Visible = true;
            }
            catch (Exception ex)
            {
                label1.Text = ex.ToString();
            }


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
            try
            {
                label4.Text = "";
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", int.Parse(textBox2.Text));
                NetworkStream networkStream = tcpClient.GetStream();
                BinaryWriter writer = new BinaryWriter(networkStream);
                writer.Write(rsa.ToXmlString(false));
                button3.Visible = false;
                button1_Click(sender, e);
            }
            catch (Exception ee)
            {
                label4.Text = "請先讓對方按匯入公鑰";
            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

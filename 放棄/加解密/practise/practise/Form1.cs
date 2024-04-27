using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace practise
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        UdpClient udp = new UdpClient(3001);
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        string op1 = "", op2 = "";
        byte[] fileBytes;
        int fileSize;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            label2.Text = openFileDialog1.FileName;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 伺服器端的IP和Port

            UdpClient udp = new UdpClient();

            try
            {
                // 讀取檔案內容
                string filePath = label2.Text;
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // 傳送檔案大小
                int fileSize = fileBytes.Length;
                byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
                send_msg_to("fileSizeBytes@" + Convert.ToBase64String(fileSizeBytes), textBox1.Text, int.Parse(textBox2.Text));

                // 傳送檔案內容
                send_msg_to("fileBytes@" + Convert.ToBase64String(fileBytes), textBox1.Text, int.Parse(textBox2.Text));

                label4.Text = "檔案傳送完成。";
            }
            catch (Exception ex)
            {
                label4.Text = $"發生錯誤: {ex.Message}";
            }
        }
            private void send_msg_to(string mydata, string ip, int port)
            {
                byte[] data = Encoding.UTF8.GetBytes(mydata);
                UdpClient udp = new UdpClient();
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                udp.Send(data, data.Length, ep);
            }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (udp.Available > 0)
            {
                    try
                    {
                        byte[] rawdata = udp.Receive(ref ep);
                        string rawstring = Encoding.UTF8.GetString(rawdata);
                        string[] token = rawstring.Split('@');
                        switch (token[0])
                        {
                            case "fileSizeBytes":
                                op1 = token[1];
                                fileSize += BitConverter.ToInt32(Convert.FromBase64String(op1),0);
                                break;
                            case "fileBytes":
                                op2 = token[1];
                                fileBytes = Convert.FromBase64String(op2);
                                break;
                        }
                        if (op1 != "" && op2 != "")
                        {
                            label5.Text = "檔案接收成功";
                        }
                    }
                    catch (Exception ex)
                    {
                        label5.Text = $"發生錯誤: {ex.Message}";
                    }
                }

        }

        private void button3_Click(object sender, EventArgs e)
        {

            try
            {
                saveFileDialog1.ShowDialog();
                string filePath = saveFileDialog1.FileName;
                string file = filePath;
                File.WriteAllBytes(filePath, fileBytes);
                label8.Text = $"檔案接收完成，大小: {fileSize} bytes，儲存於: {file}";
            }
            catch (Exception ex)
            {
                label8.Text = $"發生錯誤: {ex.Message}";
            }
        }
    }
}

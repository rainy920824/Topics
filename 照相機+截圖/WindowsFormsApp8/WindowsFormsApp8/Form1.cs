using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace WindowsFormsApp8
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        public Form1()
        {
            InitializeComponent();
            InitializeCamera();
            //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.KeyPreview = true; // 啟用窗體接收按鍵事件
            this.KeyDown += MainForm_KeyDown; // 綁定按鍵按下事件
            this.Resize += MainForm_Resize;
            int x = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            int y = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            this.Location = new Point(x, y);
        }
        private void InitializeCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("找不到攝像頭！");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);

            videoSource.Start();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            pictureBox1.Size = this.ClientSize; // Set pictureBox1 size same as form size
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (System.Drawing.Image)eventArgs.Frame.Clone();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) // 按下空格鍵拍照
            {
                TakePhoto();
            }
        }
        int pictrue = 1;
        private void TakePhoto()
        {
            if (pictureBox1.Image != null)
            {
                string filePath = @"C:\Users\rainycat\Desktop\照片測試\"+pictrue+".jpg"; // Set your desired file path here
                try
                {
                    pictureBox1.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    pictrue += 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存圖片時出錯：" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("沒有可拍攝的畫面！");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }
    }
}

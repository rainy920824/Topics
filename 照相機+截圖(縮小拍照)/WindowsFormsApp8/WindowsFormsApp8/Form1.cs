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
        private Rectangle captureRect; 

        public Form1()
        {
            InitializeComponent();
            InitializeCamera();
            InitializeCaptureRect();
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.Resize += MainForm_Resize;
            CenterForm();
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

        private void InitializeCaptureRect()
        {
            captureRect = new Rectangle(400, 200, 500, 300);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            InitializeCaptureRect();
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();

            // 在PictureBox上绘制捕获框框
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.DrawRectangle(Pens.Red, captureRect);
            }
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
                // 截取捕获框框内的图像
                
                Bitmap capturedImage = new Bitmap(captureRect.Width, captureRect.Height);
                using (Graphics g = Graphics.FromImage(capturedImage))
                {
                    g.DrawImage(pictureBox1.Image, new Rectangle(0, 0, capturedImage.Width, capturedImage.Height),
                                captureRect, GraphicsUnit.Pixel);
                }

                // 保存截取的图像
                string filePath = @"C:\Users\rainycat\Desktop\照片測試\" + pictrue + ".jpg"; // Set your desired file path here
                try
                {
                    capturedImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
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

        private void CenterForm()
        {
            int x = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            int y = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            this.Location = new Point(x, y);
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

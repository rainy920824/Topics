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
using System.Speech.Recognition;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace WindowsFormsApp9
{
    public partial class photograph : Form
    {
        SqlCommand cmdadd, cmdselect, cmdupdate, cmddelete;
        SqlDataReader reader;
        static string op;
        static string op2="1";
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Rectangle captureRect;
        private static SpeechRecognitionEngine recognizer;
        public photograph()
        {
            InitializeComponent();
            InitializeCamera();
            InitializeCaptureRect();
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
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
            Bitmap newFrame = (Bitmap)eventArgs.Frame.Clone();

            // 清除原有的 PictureBox 图像
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }

            // 将新的帧设置为 PictureBox 的图像
            pictureBox1.Image = newFrame;

            // 在 PictureBox 上绘制捕获框框
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.DrawRectangle(Pens.Red, captureRect);
            }
        }
        int no = 1;
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) // 按下空格鍵拍照
            {
                if(num==1)
                {
                    label1.Text = "語音開啟中";
                    label1.ForeColor = Color.Red;
                    TakePhoto();
                    mic();
                    num = 2;
                }else if(num==2)
                {
                    recognizer.RecognizeAsyncStop();
                    label1.Text = "語音關閉中";
                    label1.ForeColor = Color.Black;
                    //checktickets1.uuu.Text=op;      //先去別的視窗把那個物件打開public
                    // 文件夹路径
                   
                    if (op!=op2)
                    {
                        //修改照片位置
                        string folderPath = @"C:\Users\rainycat\Desktop\照片測試\";
                        string[] imagePaths = Directory.GetFiles(folderPath, pictrue+".jpg");
                        string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        string path = (System.IO.Path.GetDirectoryName(executable));
                        AppDomain.CurrentDomain.SetData("DataDirectory", path.Substring(0, path.Length - 10));
                        SqlConnection cnn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True");
                        //cmddelete = new SqlCommand("delete from election", cnn);
                        //cnn.Open();
                        //cmddelete.ExecuteNonQuery();
                        //cnn.Close();
                        cmdadd = new SqlCommand("insert into election(no, photo,voice,picture) values (@no,@photo,@voice, @imageData)", cnn);
                        cnn.Open();
                        cmdadd.Parameters.Add(new SqlParameter("@no", no));
                        cmdadd.Parameters.Add(new SqlParameter("@photo", op));
                        cmdadd.Parameters.Add(new SqlParameter("@voice", op2));
                        cmdadd.Parameters.Add(new SqlParameter("@imageData", SqlDbType.Image));
                        foreach (string imagePath in imagePaths)
                        {
                            // 读取图片数据
                            byte[] imageData = File.ReadAllBytes(imagePath);
                            cmdadd.Parameters["@imageData"].Value = imageData;
                            
                            cmdadd.ExecuteNonQuery();
                        }
                        cnn.Close();
                        no++;
                        num = 1;
                        pictrue += 1;
                        //cmdadd.Parameters.Add(new SqlParameter("@picture", SqlDbType.Image) { Value = imageData });
                    }
                }
            }
        }
        int pictrue = 1,num=1;

        private void TakePhoto()
        {
            if (pictureBox1.Image != null)
            {
                string directoryPath = @"C:\Users\rainycat\Desktop\照片測試\";
                string filePath;

                // 循环直到找到一个不存在的文件路径
                while (true)
                {
                    filePath = Path.Combine(directoryPath, pictrue + ".jpg");
                    if (!File.Exists(filePath))
                    {
                        break; // 找到不存在的文件路径时跳出循环
                    }
                    pictrue++;
                }
                // 截取捕获框框内的图像

                Bitmap capturedImage = new Bitmap(captureRect.Width, captureRect.Height);
                using (Graphics g = Graphics.FromImage(capturedImage))
                {
                    g.DrawImage(pictureBox1.Image, new Rectangle(0, 0, capturedImage.Width, capturedImage.Height),
                                captureRect, GraphicsUnit.Pixel);
                }

                // 保存截取的图像
                try
                {
                    capturedImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
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

        private void mic()
        {
            recognizer = new SpeechRecognitionEngine();
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.AppendDictation();
            Grammar grammar = new Grammar(grammarBuilder);
            recognizer.LoadGrammar(grammar);
            recognizer.SpeechRecognized += RecognizerSpeechRecognized;
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }
        
        private static void RecognizerSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null && e.Result.Confidence > 0.5)
            {
                op = e.Result.Text;
                if (op.Length > 2)
                {
                    if (op.Substring(0, 2) == "一好" || op.Substring(0, 2) == "一套" || op.Substring(0, 2) == "一號" || op.Substring(0, 2) == "一報" || op.Substring(0, 2) == "以好")
                        op = "1";
                    else if (op.Substring(0, 2) == "噩耗" || op.Substring(0, 2) == "二號" || op.Substring(0, 2) == "二報" || op.Substring(0, 1) == "，" || op.Substring(0, 2) == "何厚" || op.Substring(0, 2) == "俄少" || op.Substring(0, 2) == "二到")
                        op = "2";
                    else if (op.Substring(0, 2) == "三好" || op.Substring(0, 2) == "三號" || op.Substring(0, 2) == "商號" || op.Substring(0, 2) == "張皓" || op.Substring(0, 2) == "三報" || op.Substring(0, 2) == "單號")
                        op = "3";
                    else if (op.Substring(0, 2) == "無效" || op.Substring(0, 2) == "不肖" || op.Substring(0, 3) == "吳笑料")
                        op = "no";
                    else
                        op = "no";
                }
            }
            op = "no";          //測試
        }
    }
}

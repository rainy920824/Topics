using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //insert + listBox顯示資料在內(no, photo, voice)
        private void button1_Click(object sender, EventArgs e)
        {
            //Base64 編碼 -> 字節數組
            byte[] imageData = Convert.FromBase64String(textBox5.Text);

            //SqlConnection 建立連接
            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\User\\OneDrive\\桌面\\WindowsFormsApp1\\WindowsFormsApp1\\Database1.mdf;Integrated Security=True");
            //創建 SqlCommand 與 SQL 語句和連接
            SqlCommand cmd = new SqlCommand("INSERT INTO Data_1 VALUES (@no, @photo, @voice, @note, @picture)", conn);
            conn.Open();
            cmd.Parameters.Add(new SqlParameter("@no", textBox1.Text));
            cmd.Parameters.Add(new SqlParameter("@photo", textBox2.Text));
            cmd.Parameters.Add(new SqlParameter("@voice", textBox3.Text));
            cmd.Parameters.Add(new SqlParameter("@note", textBox4.Text));
            //圖片數據 -> SqlCommand
            cmd.Parameters.Add(new SqlParameter("@picture", SqlDbType.Image) { Value = imageData });
            cmd.ExecuteNonQuery();
            // /更新listBox
            SqlCommand selectCmd = new SqlCommand($"SELECT no, photo, voice FROM Data_1 WHERE note = 'Ture'", conn);
            SqlDataReader dr = selectCmd.ExecuteReader();
            // 清空 listBox
            listBox1.Items.Clear();
            // 添加列標題
            listBox1.Items.Add("No\tPhoto\tVoice");

            // 添加紀錄
            while (dr.Read())
            {
                //取值
                string no = dr["no"].ToString();
                string photo = dr["photo"].ToString();
                string voice = dr["voice"].ToString();

                // 格式化並加到 listBox 中
                string listItem = $"{no}\t{photo}\t{voice,-5}";
                //string listItem = $"{no,-4} {photo,-15} {voice,-5}";

                listBox1.Items.Add(listItem);
            }
                conn.Close();

            //清空
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }

        // comboBox填充項目
        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\User\\OneDrive\\桌面\\WindowsFormsApp1\\WindowsFormsApp1\\Database1.mdf;Integrated Security=True");
            SqlCommand cmd = new SqlCommand($"SELECT no, photo, voice, picture, note FROM Data_1 WHERE note = 'Ture'", conn);
            conn.Open();
            //讀取
            SqlDataReader dr = cmd.ExecuteReader();
            comboBox1.Items.Clear(); // 清空 comboBox1

            while (dr.Read())
            {
                comboBox1.Items.Add(dr["no"]); // 添加項目到 comboBox1
                                               //comboBox1.SelectedIndex = 0;
            }
            conn.Close(); // 關閉資料庫連接
        }
        
        //選取圖片檔案檔案
         private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            //開啟檔案
            if (openfile.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(openfile.FileName))
            {
                string pic = openfile.FileName;
                // 將圖片 -> 字節數組 -> Base64
                byte[] imageData = File.ReadAllBytes(pic);
                string base64String = Convert.ToBase64String(imageData);
                //textBox5.Multiline = true;
                textBox5.Text = base64String; //Base64顯示在textbos5.text中

            }

            openfile.Dispose();
        }

        //comboBox抓取資料(phtot, voice, picture)
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 確保 comboBox1 中有選擇的項目
            if (comboBox1.SelectedIndex != -1)
            {
                // 獲取所選的項目
                string selectedNo = comboBox1.SelectedItem.ToString();

                // 使用所選項目的值從資料庫中查詢相關資訊
                SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\User\\OneDrive\\桌面\\WindowsFormsApp1\\WindowsFormsApp1\\Database1.mdf;Integrated Security=True");
                SqlCommand cmd = new SqlCommand($"SELECT photo, voice, picture FROM Data_1 WHERE no = '{selectedNo}'", conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                // 如果有查詢結果
                if (dr.Read())
                {
                    // 顯示 photo 和 voice
                    label7.Text = dr["photo"].ToString();
                    label9.Text = dr["voice"].ToString();

                    // 顯示圖片
                    if (dr["picture"] != DBNull.Value)
                    {
                        byte[] imgData = (byte[])dr["picture"];
                        using (MemoryStream ms = new MemoryStream(imgData))
                        {
                            Image image = Image.FromStream(ms);
                            pictureBox1.Image = image;
                        }
                    }
                }

                conn.Close(); // 關閉資料庫連接
            }
        }
        //listBox抓取資料(phtot, voice, picture)
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selectedNo = listBox1.SelectedItem.ToString().Split('\t')[0];

                SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\User\\OneDrive\\桌面\\WindowsFormsApp1\\WindowsFormsApp1\\Database1.mdf;Integrated Security=True");
                SqlCommand cmd = new SqlCommand("SELECT photo, voice, picture FROM Data_1 WHERE no = @no", conn);
                conn.Open();
                    cmd.Parameters.AddWithValue("@no", selectedNo); // 设置 @no 参数
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        // 显示 photo 和 voice
                        label7.Text = dr["photo"].ToString();
                        label9.Text = dr["voice"].ToString();

                        // 显示 picture
                        if (dr["picture"] != DBNull.Value)
                        {
                            byte[] imgData = (byte[])dr["picture"];
                            using (MemoryStream ms = new MemoryStream(imgData))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                            }
                        }
                    }
                }
            }
        }
    }

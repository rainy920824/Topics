using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string no, photo, voice;
        SqlCommand cmdadd, cmdselect, cmdupdate, cmddelete;

        private void Form1_Load(object sender, EventArgs e)
        {
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path.Substring(0, path.Length - 10));
            //絕對路徑修改
            SqlConnection cnn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\rainycat\Desktop\Topics\成果\WindowsFormsApp9\WindowsFormsApp9\Database1.mdf;Integrated Security=True");
           
            cnn.Open();
            cmdselect = new SqlCommand("SELECT * FROM election", cnn);
            reader = cmdselect.ExecuteReader();
            listBox1.Items.Clear();
            listBox2.Items.Insert(0, "編號\t\t\t圖片辨識結果\t\t語音辨識結果");
            //listBox1.Items.Add("編號\t\t\t語音辨識結果\t\t圖片辨識結果");
            while (reader.Read())
            {
                // 讀取查詢結果
                //取值
                string no = reader["no"].ToString();
                string photo = reader["photo"].ToString();
                string voice = reader["voice"].ToString();
                if (int.Parse(no) > 0 && int.Parse(no) < 10)
                    no = "0" + no;
                if (photo == "no   ")
                    photo = "無效";
                if (photo == "1    " || photo == "2    " || photo == "3    ")
                    photo = "   " + photo;
                if (voice == "no   ")
                    voice = "無效";
                if (voice == "1    " || photo == "2    " || photo == "3    ")
                    voice = "   " + voice;
                // 格式化並加到 listBox 中
                string listItem = $"  "+no+ "\t\t\t\n\n\n\n\n\n" + photo+ "\t\t\t\n\n\n\n\n\n\n" + voice;
                //string listItem = $"{no,-4} {photo,-15} {voice,-5}";

                listBox1.Items.Add(listItem);
            }
            cnn.Close();
        }

        SqlDataReader reader;
        public Form1()
        {
            InitializeComponent();
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selectedNo = listBox1.SelectedItem.ToString().Split('\t')[0];
                //絕對路徑修改
                SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\rainycat\Desktop\Topics\成果\WindowsFormsApp9\WindowsFormsApp9\Database1.mdf;Integrated Security=True");
                SqlCommand cmd = new SqlCommand("SELECT* FROM election WHERE no = @no", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("@no", selectedNo);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
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


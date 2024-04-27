using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // 設定 Azure 圖片預測服務的訂閱金鑰和端點

        // 預設的檔案位置
        int num = 1;
        private async void button1_Click(object sender, EventArgs e)
        {
            
            string predictionKey = "9633831a76db4504acb73519d2a49c2d";
            string imagePath = @"C:\Users\rainy\OneDrive\桌面\專\"+num+".png";
            try
            {
                // 讀取圖像檔案的位元組數據
                byte[] imageBytes = File.ReadAllBytes(imagePath);

                // 設定請求
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, "https://113008241-prediction.cognitiveservices.azure.com/customvision/v3.0/Prediction/7f8fad0a-a5ce-4114-844d-15ac6f19d5b2/detect/iterations/Iteration1/image"))
                    {
                        // 設定標頭
                        request.Headers.Add("Prediction-Key", predictionKey.ToLower());
                        request.Content = new ByteArrayContent(imageBytes);
                        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        // 設定正文
                        request.Content = new ByteArrayContent(imageBytes);

                        // 發送請求
                        using (var response = await httpClient.SendAsync(request))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                // 處理回應
                                string responseBody = await response.Content.ReadAsStringAsync();
                                string json = responseBody;
                                JObject jsonObject = JObject.Parse(json);
                                string tag = jsonObject["predictions"][0]["tagName"].ToString();
                                double probability = Convert.ToDouble(jsonObject["predictions"][0]["probability"]);
                                label1.Text = "標籤:"+tag;
                                label2.Text = "概率:" +probability;
                                num += 1;
                            }
                            else
                            {
                                MessageBox.Show($"Error: {response.StatusCode}", "Error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }
    }
}

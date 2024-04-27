using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Speech.Recognition;

namespace ConsoleApp2
{
    class Program
    {
        static void Main()
        {
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

            // 設定語音辨識引擎使用的語法
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.AppendDictation(); // 使用內建的語法，支援一般的語音辨識

            System.Speech.Recognition.Grammar grammar = new System.Speech.Recognition.Grammar(grammarBuilder);

            recognizer.LoadGrammar(grammar);

            // 設定語音辨識完成後的事件處理程序
            recognizer.SpeechRecognized += RecognizerSpeechRecognized;

            // 啟動語音辨識引擎
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            Console.WriteLine("請講話...");
            Console.ReadLine(); // 等待用戶輸入

            // 停止語音辨識引擎
            recognizer.RecognizeAsyncStop();
        }
        private static void RecognizerSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string op = e.Result.Text;
            if (op.Length > 2)
            {
                if (op.Substring(0, 2) == "一好" || op.Substring(0, 2) == "一套" || op.Substring(0, 2) == "一號" || op.Substring(0, 2) == "一報")
                    op = "一號一票";
                else if (op.Substring(0, 2) == "噩耗" || op.Substring(0, 2) == "二號" || op.Substring(0, 2) == "二報")
                    op = "二號一票";
                else if (op.Substring(0, 2) == "三好" || op.Substring(0, 2) == "三號" || op.Substring(0, 2) == "商號" || op.Substring(0, 2) == "張皓" || op.Substring(0, 2) == "三報")
                    op = "三號一票";
                else
                    op = e.Result.Text;
            }
            else
                op = e.Result.Text;

            Console.WriteLine("辨識結果:" + op);
        }
    }
}

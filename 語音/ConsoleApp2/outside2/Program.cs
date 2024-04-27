using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;

namespace outside2
{
    class Program
    {
        private static SpeechRecognitionEngine recognizer;
        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine("按下空白鍵開始語音辨識，按下空白鍵暫停...");
            while (i != 1)
            {
                while (Console.ReadKey().Key != ConsoleKey.Spacebar)
                {
                    Console.WriteLine("按下空白鍵開始語音辨識，再次按下空白鍵暫停...");
                }
                recognizer = new SpeechRecognitionEngine();
                GrammarBuilder grammarBuilder = new GrammarBuilder();
                grammarBuilder.AppendDictation();
                Grammar grammar = new Grammar(grammarBuilder);
                recognizer.LoadGrammar(grammar);
                recognizer.SpeechRecognized += RecognizerSpeechRecognized;
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                Console.WriteLine("請講話...");
                if(Console.ReadKey().Key == ConsoleKey.Spacebar)
                {
                    recognizer.RecognizeAsyncStop();
                    Console.WriteLine("已暫停，請按下空白鍵繼續語音辨識");
                }
            }

        }
        private static void RecognizerSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null && e.Result.Confidence > 0.5)
            {
                string op = e.Result.Text;
                if (op.Length > 2)
                {
                    if (op.Substring(0, 2) == "一好" || op.Substring(0, 2) == "一套" || op.Substring(0, 2) == "一號" || op.Substring(0, 2) == "一報" || op.Substring(0, 2) == "以好")
                        op = "一號一票";
                    else if (op.Substring(0, 2) == "噩耗" || op.Substring(0, 2) == "二號" || op.Substring(0, 2) == "二報" || op.Substring(0, 1) == "，" || op.Substring(0, 2) == "何厚" || op.Substring(0, 2) == "俄少" || op.Substring(0, 2) == "二到")
                        op = "二號一票";
                    else if (op.Substring(0, 2) == "三好" || op.Substring(0, 2) == "三號" || op.Substring(0, 2) == "商號" || op.Substring(0, 2) == "張皓" || op.Substring(0, 2) == "三報" || op.Substring(0, 2) == "單號")
                        op = "三號一票";
                    else if (op.Substring(0,2) == "無效" || op.Substring(0, 2) == "不肖" || op.Substring(0, 3) == "吳笑料")
                        op = "無效票一票";
                    else
                        op = e.Result.Text;
                }
                else
                    op = e.Result.Text;

                Console.WriteLine("辨識結果:" + op);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace ConsoleApp1
{
    class Program
    {
        async static Task FromMic(SpeechConfig speechConfig)
        {
            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            Console.WriteLine("Speak into your microphone.");
            var result = await speechRecognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task Main(string[] args)
        {
            var speechConfig = SpeechConfig.FromSubscription("YourSpeechKey", "YourSpeechRegion");
            await FromMic(speechConfig);
        }
    }
}

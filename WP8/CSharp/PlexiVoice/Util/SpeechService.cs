using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

using Windows.Foundation;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis; 

namespace PlexiVoice.Util
{
    class SpeechService : ISpeechService
    {
        private SpeechSynthesizer synthesizer;

        private SpeechRecognizerUI recognizer;

        bool disableSpeech = false;

        public bool isRecording { get; set; }

        public SpeechService()
        {
            try
            {
                if (synthesizer == null)
                {
                    synthesizer = new SpeechSynthesizer();
                }

                if (recognizer == null)
                {
                    recognizer = new SpeechRecognizerUI();
                    
                    //recognizer.Recognizer.Settings.BabbleTimeout = TimeSpan.FromSeconds(5);
                    recognizer.Settings.ReadoutEnabled = false;
                    //recognizer.Settings.ShowConfirmation = false;
                }

                isRecording = false;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }


        public async Task<string> PerformSpeechRecognition()
        {
            try
            {
                CancelSpeak();

                disableSpeech = false;
            
                isRecording = true;

                var recoResult = await recognizer.RecognizeWithUIAsync();

                // reset the mic button if the user cancels the recognition gui
                if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Cancelled)
                {
                    isRecording = false;
                }
                else if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    string query = recoResult.RecognitionResult.Text;

                    // replace profanity text
                    query = Regex.Replace(query, @"<profanity>(.*?)</profanity>", new MatchEvaluator(ProfanityFilter), RegexOptions.IgnoreCase);

                    // is this check needed in a succeeded state
                    if (recoResult.RecognitionResult.TextConfidence == SpeechRecognitionConfidence.Rejected)
                    {
                        //Say("please", "I didn't quite catch that. Can you say it again?");
                    }
                    else
                    {
                        //TODO: conditional is needed
                        // check if speak is in response to a task filter ie. narrow down results for a phone task like email or sms
                        // if (this.GetType() == typeof(ContactList))

                        isRecording = false;

                        return query;
                    }
                }
            }
            catch (Exception err)
            {
                const int privacyPolicyHResult = unchecked((int)0x80045509);
               
                if (err.HResult == privacyPolicyHResult)
                {
                    MessageBox.Show("To run this sample, you must first accept the speech privacy policy. To do so, navigate to Settings -> speech on your phone and check 'Enable Speech Recognition Service' ");
                }
                else
                {
                    Debug.WriteLine(err.Message);
                }
            }

            return null;
        }

        public async Task Speak(DialogOwner owner, string speak = "")
        {
            // say response
            if (owner == DialogOwner.Plexi)
            {
                await Speak(speak);
            }
        }

        public async Task Speak(string speak = "")
        {
            try
            {
                //if (speak != "" && disableSpeech != false)
                if (speak != "")
                {
                    await synthesizer.SpeakTextAsync(speak);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        public void CancelSpeak()
        {
            synthesizer.CancelAll();
        }

        private static string ProfanityFilter(Match match)
        {
            string replacement = "";

            for (var i = 1; i < match.Groups.Count; i++)
            {
                for (var j = 0; j < match.Groups[i].Length; j++)
                {
                    replacement += "*";
                }
            }

            return replacement;
        }
    }
}

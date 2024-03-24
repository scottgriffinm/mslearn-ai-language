using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;


/*


Translate Speech
Azure AI Speech includes a speech translation API that you can use to translate spoken language. For example, suppose you want to develop a translator application that people can use when traveling in places where they don't speak the local language. They would be able to say phrases such as "Where is the station?" or "I need to find a pharmacy" in their own language, and have it translate them to the local language.

NOTE This exercise requires that you are using a computer with speakers/headphones. For the best experience, a microphone is also required. Some hosted virtual environments may be able to capture audio from your local microphone, but if this doesn't work (or you don't have a microphone at all), you can use a provided audio file for speech input. Follow the instructions carefully, as you'll need to choose different options depending on whether you are using a microphone or the audio file.

Provision an Azure AI Speech resource
If you don't already have one in your subscription, you'll need to provision an Azure AI Speech resource.

Open the Azure portal at https://portal.azure.com, and sign in using the Microsoft account associated with your Azure subscription.
In the search field at the top, search for Azure AI services and press Enter, then select Create under Speech service in the results.
Create a resource with the following settings:
Subscription: Your Azure subscription
Resource group: Choose or create a resource group
Region: Choose any available region
Name: Enter a unique name
Pricing tier: Select F0 (free), or S (standard) if F is not available.
Responsible AI Notice: Agree.
Select Review + create.
Wait for deployment to complete, and then go to the deployed resource.
View the Keys and Endpoint page. You will need the information on this page later in the exercise.
Prepare to develop an app in Visual Studio Code
You'll develop your speech app using Visual Studio Code. The code files for your app have been provided in a GitHub repo.

Tip: If you have already cloned the mslearn-ai-language repo, open it in Visual Studio code. Otherwise, follow these steps to clone it to your development environment.

Start Visual Studio Code.

Open the palette (SHIFT+CTRL+P) and run a Git: Clone command to clone the https://github.com/MicrosoftLearning/mslearn-ai-language repository to a local folder (it doesn't matter which folder).

When the repository has been cloned, open the folder in Visual Studio Code.

Wait while additional files are installed to support the C# code projects in the repo.

Note: If you are prompted to add required assets to build and debug, select Not Now.

Configure your application
Applications for both C# and Python have been provided. Both apps feature the same functionality. First, you'll complete some key parts of the application to enable it to use your Azure AI Speech resource.

In Visual Studio Code, in the Explorer pane, browse to the Labfiles/08-speech-translation folder and expand the CSharp or Python folder depending on your language preference and the translator folder it contains. Each folder contains the language-specific code files for an app into which you're you're going to integrate Azure AI Speech functionality.

Right-click the translator folder containing your code files and open an integrated terminal. Then install the Azure AI Speech SDK package by running the appropriate command for your language preference:

C#

dotnet add package Microsoft.CognitiveServices.Speech --version 1.30.0
Python

pip install azure-cognitiveservices-speech==1.30.0
In the Explorer pane, in the translator folder, open the configuration file for your preferred language

C#: appsettings.json
Python: .env
Update the configuration values to include the region and a key from the Azure AI Speech resource you created (available on the Keys and Endpoint page for your Azure AI Speech resource in the Azure portal).

NOTE: Be sure to add the region for your resource, not the endpoint!

Save the configuration file.


Save your changes and return to the integrated terminal for the translator folder, and enter the following command to run the program:

C#

dotnet run
Python

python translator.py
When prompted, enter a valid language code (fr, es, or hi), and then speak clearly into the microphone and say a phrase you might use when traveling abroad. The program should transcribe your spoken input and respond with a spoken translation. Repeat this process, trying each language supported by the application. When you're finished, press ENTER to end the program.


*/



// Import namespaces
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

namespace speech_translation
{
    class Program
    {
        private static SpeechConfig speechConfig;
        private static SpeechTranslationConfig translationConfig;

        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcKey = configuration["SpeechKey"];
                string aiSvcRegion = configuration["SpeechRegion"];

                // Set console encoding to unicode
                Console.InputEncoding = Encoding.Unicode;
                Console.OutputEncoding = Encoding.Unicode;


                // Configure translation
                translationConfig = SpeechTranslationConfig.FromSubscription(aiSvcKey, aiSvcRegion);
                translationConfig.SpeechRecognitionLanguage = "en-US";
                translationConfig.AddTargetLanguage("fr");
                translationConfig.AddTargetLanguage("es");
                translationConfig.AddTargetLanguage("hi");
                Console.WriteLine("Ready to translate from " + translationConfig.SpeechRecognitionLanguage);

                // Configure speech
                speechConfig = SpeechConfig.FromSubscription(aiSvcKey, aiSvcRegion);

                string targetLanguage = "";
                while (targetLanguage != "quit")
                {
                    Console.WriteLine("\nEnter a target language\n fr = French\n es = Spanish\n hi = Hindi\n Enter anything else to stop\n");
                    targetLanguage = Console.ReadLine().ToLower();
                    if (translationConfig.TargetLanguages.Contains(targetLanguage))
                    {
                        await Translate(targetLanguage);
                    }
                    else
                    {
                        targetLanguage = "quit";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task Translate(string targetLanguage)
        {
            string translation = "";

            // Translate speech
            using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using TranslationRecognizer translator = new TranslationRecognizer(translationConfig, audioConfig);
            Console.WriteLine("Speak now...");
            TranslationRecognitionResult result = await translator.RecognizeOnceAsync();
            Console.WriteLine($"Translating '{result.Text}'");
            translation = result.Translations[targetLanguage];
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(translation);

            // Synthesize translation
            var voices = new Dictionary<string, string>
            {
                ["fr"] = "fr-FR-HenriNeural",
                ["es"] = "es-ES-ElviraNeural",
                ["hi"] = "hi-IN-MadhurNeural"
            };
            speechConfig.SpeechSynthesisVoiceName = voices[targetLanguage];
            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);
            SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(translation);
            if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine(speak.Reason);
            }

        }

    }
}

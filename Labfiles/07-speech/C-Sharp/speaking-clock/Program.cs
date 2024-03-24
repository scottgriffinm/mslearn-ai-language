using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


/*Recognize and synthesize speech
Azure AI Speech is a service that provides speech-related functionality, including:

A speech-to-text API that enables you to implement speech recognition (converting audible spoken words into text).
A text-to-speech API that enables you to implement speech synthesis (converting text into audible speech).
In this exercise, you'll use both of these APIs to implement a speaking clock application.

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

In Visual Studio Code, in the Explorer pane, browse to the Labfiles/07-speech folder and expand the CSharp or Python folder depending on your language preference and the speaking-clock folder it contains. Each folder contains the language-specific code files for an app into which you're you're going to integrate Azure AI Speech functionality.

Right-click the speaking-clock folder containing your code files and open an integrated terminal. Then install the Azure AI Speech SDK package by running the appropriate command for your language preference:

C#

dotnet add package Microsoft.CognitiveServices.Speech --version 1.30.0
Python

pip install azure-cognitiveservices-speech==1.30.0
In the Explorer pane, in the speaking-clock folder, open the configuration file for your preferred language

C#: appsettings.json
Python: .env
Update the configuration values to include the region and a key from the Azure AI Speech resource you created (available on the Keys and Endpoint page for your Azure AI Speech resource in the Azure portal).

NOTE: Be sure to add the region for your resource, not the endpoint!

Save the configuration file.

Add code to use the Azure AI Speech SDK
Note that the speaking-clock folder contains a code file for the client application:

C#: Program.cs
Python: speaking-clock.py
Open the code file and at the top, under the existing namespace references, find the comment Import namespaces. Then, under this comment, add the following language-specific code to import the namespaces you will need to use the Azure AI Speech SDK:

C#: Program.cs

csharp
// Import namespaces
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
Python: speaking-clock.py

python
# Import namespaces
import azure.cognitiveservices.speech as speech_sdk
In the Main function, note that code to load the service key and region from the configuration file has already been provided. You must use these variables to create a SpeechConfig for your Azure AI Speech resource. Add the following code under the comment Configure speech service:

C#: Program.cs

csharp
// Configure speech service
speechConfig = SpeechConfig.FromSubscription(aiSvcKey, aiSvcRegion);
Console.WriteLine("Ready to use speech service in " + speechConfig.Region);

// Configure voice
speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";
Python: speaking-clock.py

python
# Configure speech service
speech_config = speech_sdk.SpeechConfig(ai_key, ai_region)
print('Ready to use speech service in:', speech_config.region)
Save your changes and return to the integrated terminal for the speaking-clock folder, and enter the following command to run the program:

C#

dotnet run
Python

python speaking-clock.py
If you are using C#, you can ignore any warnings about using the await operator in asynchronous methods - we'll fix that later. The code should display the region of the speech service resource the application will use.

Add code to recognize speech
Now that you have a SpeechConfig for the speech service in your Azure AI Speech resource, you can use the Speech-to-text API to recognize speech and transcribe it to text.

IMPORTANT: This section includes instructions for two alternative procedures. Follow the first procedure if you have a working microphone. Follow the second procedure if you want to simulate spoken input by using an audio file.

If you have a working microphone
In the Main function for your program, note that the code uses the TranscribeCommand function to accept spoken input.

In the TranscribeCommand function, under the comment Configure speech recognition, add the appropriate code below to create a SpeechRecognizer client that can be used to recognize and transcribe speech using the default system microphone:

C#
Alternatively, use audio input from a file
In the terminal window, enter the following command to install a library that you can use to play the audio file:

C#
Save your changes and return to the integrated terminal for the speaking-clock folder, and enter the following command to run the program:

C#

dotnet run
Python

python speaking-clock.py
When prompted, speak clearly into the microphone and say "what time is it?". The program should speak in the voice that is specified in the SSML (overriding the voice specified in the SpeechConfig), telling you the time, and then after a pause, telling you it's time to end this lab - which it is!


*/

// Import namespaces
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Media;
using System.Collections.Concurrent;

namespace speaking_clock
{
    class Program
    {
        private static SpeechConfig speechConfig;
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcKey = configuration["SpeechKey"];
                string aiSvcRegion = configuration["SpeechRegion"];

                // Configure speech service
                speechConfig = SpeechConfig.FromSubscription(aiSvcKey, aiSvcRegion);
                Console.WriteLine("Ready to use speech service in " + speechConfig.Region);
                // Configure voice
                speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";
                // Get spoken input
                string command = "";
                command = await TranscribeCommand();
                if (command.ToLower() == "what time is it?")
                {
                    await TellTime();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task<string> TranscribeCommand()
        {
            string command = "";

            // Configure speech recognition
            using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
            Console.WriteLine("Speak now...");

            // Process speech input

            SpeechRecognitionResult speech = await speechRecognizer.RecognizeOnceAsync();
            if (speech.Reason == ResultReason.RecognizedSpeech)
            {
                command = speech.Text;
                Console.WriteLine(command);
            }
            else
            {
                Console.WriteLine(speech.Reason);
                if (speech.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(speech);
                    Console.WriteLine(cancellation.Reason);
                    Console.WriteLine(cancellation.ErrorDetails);
                }
            }

            // Return the command
            return command;
        }

        static async Task TellTime()
        {
            var now = DateTime.Now;
            string responseText = "The time is " + now.Hour.ToString() + ":" + now.Minute.ToString("D2");

            // Configure speech synthesis
            speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

            // Synthesize spoken output
            SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(responseText);
            if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine(speak.Reason);
            }

            // Print the response
            Console.WriteLine(responseText);
        }

    }
}

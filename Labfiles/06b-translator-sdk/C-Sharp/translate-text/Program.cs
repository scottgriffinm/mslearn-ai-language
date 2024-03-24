using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;


/*

You will need an Azure subscription. If you don't already have one, you can sign up for one that includes free credits for the first 30 days.

Sign into Windows as Student account with the password Pa55w.rd.
Follow the instructions below to complete the exercise.
Translate Text
Azure AI Translator is a service that enables you to translate text between languages. In this exercise, you'll use it to create a simple app that translates input in any supported language to the target language of your choice.

Provision an Azure AI Translator resource
If you don't already have one in your subscription, you'll need to provision an Azure AI Translator resource.

Open the Azure portal at https://portal.azure.com, and sign in using the Microsoft account associated with your Azure subscription.
In the search field at the top, search for Azure AI services and press Enter, then select Create under Translator in the results.
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
You'll develop your text translation app using Visual Studio Code. The code files for your app have been provided in a GitHub repo.

Tip: If you have already cloned the mslearn-ai-language repo, open it in Visual Studio code. Otherwise, follow these steps to clone it to your development environment.

Start Visual Studio Code.

Open the palette (SHIFT+CTRL+P) and run a Git: Clone command to clone the https://github.com/MicrosoftLearning/mslearn-ai-language repository to a local folder (it doesn't matter which folder).

When the repository has been cloned, open the folder in Visual Studio Code.

Wait while additional files are installed to support the C# code projects in the repo.

Note: If you are prompted to add required assets to build and debug, select Not Now.

Configure your application
Applications for both C# and Python have been provided. Both apps feature the same functionality. First, you'll complete some key parts of the application to enable it to use your Azure AI Translator resource.

In Visual Studio Code, in the Explorer pane, browse to the Labfiles/06b-translator-sdk folder and expand the CSharp or Python folder depending on your language preference and the translate-text folder it contains. Each folder contains the language-specific code files for an app into which you're you're going to integrate Azure AI Translator functionality.

Right-click the translate-text folder containing your code files and open an integrated terminal. Then install the Azure AI Translator SDK package by running the appropriate command for your language preference:

C#:

dotnet add package Azure.AI.Translation.Text --version 1.0.0-beta.1
Python:

pip install azure-ai-translation-text==1.0.0b1
In the Explorer pane, in the translate-text folder, open the configuration file for your preferred language

C#: appsettings.json
Python: .env
Update the configuration values to include the region and a key from the Azure AI Translator resource you created (available on the Keys and Endpoint page for your Azure AI Translator resource in the Azure portal).

NOTE: Be sure to add the region for your resource, not the endpoint!

Save the configuration file.
Test your application
Now your application is ready to test.

In the integrated terminal for the Translate text folder, and enter the following command to run the program:

C#: dotnet run
Python: python translate.py
Tip: You can use the Maximize panel size (^) icon in the terminal toolbar to see more of the console text.

When prompted, enter a valid target language from the list displayed.

Enter a phrase to be translated (for example This is a test or C'est un test) and view the results, which should detect the source language and translate the text to the target language.

When you're done, enter quit. You can run the application again and choose a different target language.


*/


// import namespaces
using Azure;
using Azure.AI.Translation.Text;


namespace translate_text
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Set console encoding to unicode
                Console.InputEncoding = Encoding.Unicode;
                Console.OutputEncoding = Encoding.Unicode;

                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string translatorRegion = configuration["TranslatorRegion"];
                string translatorKey = configuration["TranslatorKey"];


                // Create client using endpoint and key
AzureKeyCredential credential = new(translatorKey);
TextTranslationClient client = new(credential, translatorRegion);


                // Choose target language
Response<GetLanguagesResult> languagesResponse = await client.GetLanguagesAsync(scope:"translation").ConfigureAwait(false);
GetLanguagesResult languages = languagesResponse.Value;
Console.WriteLine($"{languages.Translation.Count} languages available.\n(See https://learn.microsoft.com/azure/ai-services/translator/language-support#translation)");
Console.WriteLine("Enter a target language code for translation (for example, 'en'):");
string targetLanguage = "xx";
bool languageSupported = false;
while (!languageSupported)
{
    targetLanguage = Console.ReadLine();
    if (languages.Translation.ContainsKey(targetLanguage))
    {
        languageSupported = true;
    }
    else
    {
        Console.WriteLine($"{targetLanguage} is not a supported language.");
    }

}


                // Translate text
// Translate text
string inputText = "";
while (inputText.ToLower() != "quit")
{
    Console.WriteLine("Enter text to translate ('quit' to exit)");
    inputText = Console.ReadLine();
    if (inputText.ToLower() != "quit")
    {
        Response<IReadOnlyList<TranslatedTextItem>> translationResponse = await client.TranslateAsync(targetLanguage, inputText).ConfigureAwait(false);
        IReadOnlyList<TranslatedTextItem> translations = translationResponse.Value;
        TranslatedTextItem translation = translations[0];
        string sourceLanguage = translation?.DetectedLanguage?.Language;
        Console.WriteLine($"'{inputText}' translated from {sourceLanguage} to {translation?.Translations[0].To} as '{translation?.Translations?[0]?.Text}'.");
    }
} 

                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



    }
}

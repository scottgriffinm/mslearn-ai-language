using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Serialization;

/*If you don't already have one in your subscription, you'll need to provision an Azure AI Language service resource in your Azure subscription.

Open the Azure portal at https://portal.azure.com, and sign in using the Microsoft account associated with your Azure subscription.
In the search field at the top, search for Azure AI services. Then, in the results, select Create under Language Service.
Select Continue to create your resource.
Provision the resource using the following settings:
Subscription: Your Azure subscription.
Resource group: Choose or create a resource group.
Region:Choose any available region
Name: Enter a unique name.
Pricing tier: Select F0 (free), or S (standard) if F is not available.
Responsible AI Notice: Agree.
Select Review + create.
Wait for deployment to complete, and then go to the deployed resource.
View the Keys and Endpoint page. You will need the information on this page later in the exercise.
Create a conversational language understanding project
Now that you have created an authoring resource, you can use it to create a conversational language understanding project.

In a new browser tab, open the Azure AI Language Studio portal at https://language.cognitive.azure.com/ and sign in using the Microsoft account associated with your Azure subscription.

If prompted to choose a Language resource, select the following settings:

Azure Directory: The Azure directory containing your subscription.
Azure subscription: Your Azure subscription.
Resource type: Language.
Language resource: The Azure AI Language resource you created previously.
If you are not prompted to choose a language resource, it may be because you have multiple Language resources in your subscription; in which case:

On the bar at the top if the page, select the Settings (⚙) button.
On the Settings page, view the Resources tab.
Select the language resource you just created, and click Switch resource.
At the top of the page, click Language Studio to return to the Language Studio home page
At the top of the portal, in the Create new menu, select Conversational language understanding.

In the Create a project dialog box, on the Enter basic information page, enter the following details and then select Next:

Name: Clock
Utterances primary language: English
Enable multiple languages in project?: Unselected
Description: Natural language clock
On the Review and finish page, select Create.

Create intents
The first thing we'll do in the new project is to define some intents. The model will ultimately predict which of these intents a user is requesting when submitting a natural language utterance.

Tip: When working on your project, if some tips are displayed, read them and select Got it to dismiss them, or select Skip all.

On the Schema definition page, on the Intents tab, select ＋ Add to add a new intent named GetTime.
Verify that the GetTime intent is listed (along with the default None intent). Then add the following additional intents:
GetDay
GetDate
Label each intent with sample utterances
To help the model predict which intent a user is requesting, you must label each intent with some sample utterances.

In the pane on the left, select the Data Labeling page.
Tip: You can expand the pane with the >> icon to see the page names, and hide it again with the << icon.

Select the new GetTime intent and enter the utterance what is the time?. This adds the utterance as sample input for the intent.

Add the following additional utterances for the GetTime intent:

what's the time?
what time is it?
tell me the time
Select the GetDay intent and add the following utterances as example input for that intent:

what day is it?
what's the day?
what is the day today?
what day of the week is it?
Select the GetDate intent and add the following utterances for it:

what date is it?
what's the date?
what is the date today?
what's today's date?
After you've added utterances for each of your intents, select Save changes.

Train and test the model
Now that you've added some intents, let's train the language model and see if it can correctly predict them from user input.

In the pane on the left, select Training jobs. Then select + Start a training job.

On the Start a training job dialog, select the option to train a new model, name it Clock. Select Standard training mode and the default Data splitting options.

To begin the process of training your model, select Train.

When training is complete (which may take several minutes) the job Status will change to Training succeeded.

Select the Model performance page, and then select the Clock model. Review the overall and per-intent evaluation metrics (precision, recall, and F1 score) and the confusion matrix generated by the evaluation that was performed when training (note that due to the small number of sample utterances, not all intents may be included in the results).

NOTE To learn more about the evaluation metrics, refer to the documentation

Go to the Deploying a model page, then select Add deployment.

On the Add deployment dialog, select Create a new deployment name, and then enter production.

Select the Clock model in the Model field then select Deploy. The deployment may take some time.

When the model has been deployed, select the Testing deployments page, then select the production deployment in the Deployment name field.

Enter the following text in the empty textbox, and then select Run the test:

what's the time now?

Review the result that is returned, noting that it includes the predicted intent (which should be GetTime) and a confidence score that indicates the probability the model calculated for the predicted intent. The JSON tab shows the comparative confidence for each potential intent (the one with the highest confidence score is the predicted intent)

Clear the text box, and then run another test with the following text:

tell me the time

Again, review the predicted intent and confidence score.

Try the following text:

what's the day today?

Hopefully the model predicts the GetDay intent.

Add entities
So far you've defined some simple utterances that map to intents. Most real applications include more complex utterances from which specific data entities must be extracted to get more context for the intent.

Add a learned entity
The most common kind of entity is a learned entity, in which the model learns to identify entity values based on examples.

In Language Studio, return to the Schema definition page and then on the Entities tab, select ＋ Add to add a new entity.

In the Add an entity dialog box, enter the entity name Location and ensure that the Learned tab is selected. Then select Add entity.

After the Location entity has been created, return to the Data labeling page.

Select the GetTime intent and enter the following new example utterance:

what time is it in London?

When the utterance has been added, select the word London, and in the drop-down list that appears, select Location to indicate that "London" is an example of a location.

Add another example utterance for the GetTime intent:

Tell me the time in Paris?

When the utterance has been added, select the word Paris, and map it to the Location entity.

Add another example utterance for the GetTime intent:

what's the time in New York?

When the utterance has been added, select the words New York, and map them to the Location entity.

Select Save changes to save the new utterances.

Add a list entity
In some cases, valid values for an entity can be restricted to a list of specific terms and synonyms; which can help the app identify instances of the entity in utterances.

In Language Studio, return to the Schema definition page and then on the Entities tab, select ＋ Add to add a new entity.

In the Add an entity dialog box, enter the entity name Weekday and select the List entity tab. Then select Add entity.

On the page for the Weekday entity, in the Learned section, ensure Not required is selected. Then, in the List section, select ＋ Add new list. Then enter the following value and synonym and select Save:

List key	synonyms
Sunday	Sun
Repeat the previous step to add the following list components:

Value	synonyms
Monday	Mon
Tuesday	Tue, Tues
Wednesday	Wed, Weds
Thursday	Thur, Thurs
Friday	Fri
Saturday	Sat
After adding and saving the list values, return to the Data labeling page.

Select the GetDate intent and enter the following new example utterance:

what date was it on Saturday?

When the utterance has been added, select the word Saturday, and in the drop-down list that appears, select Weekday.

Add another example utterance for the GetDate intent:

what date will it be on Friday?

When the utterance has been added, map Friday to the Weekday entity.

Add another example utterance for the GetDate intent:

what will the date be on Thurs?

When the utterance has been added, map Thurs to the Weekday entity.

select Save changes to save the new utterances.

Add a prebuilt entity
The Azure AI Language service provides a set of prebuilt entities that are commonly used in conversational applications.

In Language Studio, return to the Schema definition page and then on the Entities tab, select ＋ Add to add a new entity.

In the Add an entity dialog box, enter the entity name Date and select the Prebuilt entity tab. Then select Add entity.

On the page for the Date entity, in the Learned section, ensure Not required is selected. Then, in the Prebuilt section, select ＋ Add new prebuilt.

In the Select prebuilt list, select DateTime and then select Save.

After adding the prebuilt entity, return to the Data labeling page

Select the GetDay intent and enter the following new example utterance:

what day was 01/01/1901?

When the utterance has been added, select 01/01/1901, and in the drop-down list that appears, select Date.

Add another example utterance for the GetDay intent:

what day will it be on Dec 31st 2099?

When the utterance has been added, map Dec 31st 2099 to the Date entity.

Select Save changes to save the new utterances.

Retrain the model
Now that you've modified the schema, you need to retrain and retest the model.

On the Training jobs page, select Start a training job.

On the Start a training job dialog, select overwrite an existing model and specify the Clock model. Select Train to train the model. If prompted, confirm you want to overwrite the existing model.

When training is complete the job Status will update to Training succeeded.

Select the Model performance page and then select the Clock model. Review the evaluation metrics (precision, recall, and F1 score) and the confusion matrix generated by the evaluation that was performed when training (note that due to the small number of sample utterances, not all intents may be included in the results).

On the Deploying a model page, select Add deployment.

On the Add deployment dialog, select Override an existing deployment name, and then select production.

Select the Clock model in the Model field and then select Deploy to deploy it. This may take some time.

When the model is deployed, on the Testing deployments page, select the production deployment under the Deployment name field, and then test it with the following text:

what's the time in Edinburgh?

Review the result that is returned, which should hopefully predict the GetTime intent and a Location entity with the text value "Edinburgh".

Try testing the following utterances:

what time is it in Tokyo?

what date is it on Friday?

what's the date on Weds?

what day was 01/01/2020?

what day will Mar 7th 2030 be?

Use the model from a client app
In a real project, you'd iteratively refine intents and entities, retrain, and retest until you are satisfied with the predictive performance. Then, when you've tested it and are satisfied with its predictive performance, you can use it in a client app by calling its REST interface or a runtime-specific SDK.

Prepare to develop an app in Visual Studio Code
You'll develop your language understanding app using Visual Studio Code. The code files for your app have been provided in a GitHub repo.

Tip: If you have already cloned the mslearn-ai-language repo, open it in Visual Studio code. Otherwise, follow these steps to clone it to your development environment.

Start Visual Studio Code.

Open the palette (SHIFT+CTRL+P) and run a Git: Clone command to clone the https://github.com/MicrosoftLearning/mslearn-ai-language repository to a local folder (it doesn't matter which folder).

When the repository has been cloned, open the folder in Visual Studio Code.

Wait while additional files are installed to support the C# code projects in the repo.

Note: If you are prompted to add required assets to build and debug, select Not Now.

Configure your application
Applications for both C# and Python have been provided, as well as a sample text file you'll use to test the summarization. Both apps feature the same functionality. First, you'll complete some key parts of the application to enable it to use your Azure AI Language resource.

In Visual Studio Code, in the Explorer pane, browse to the Labfiles/03-language folder and expand the CSharp or Python folder depending on your language preference and the clock-client folder it contains. Each folder contains the language-specific files for an app into which you're you're going to integrate Azure AI Language question answering functionality.

Right-click the clock-client folder containing your code files and open an integrated terminal. Then install the Azure AI Language conversational language understanding SDK package by running the appropriate command for your language preference:

C#:

dotnet add package Azure.AI.Language.Conversations --version 1.1.0
Python:

pip install azure-ai-language-conversations
In the Explorer pane, in the clock-client folder, open the configuration file for your preferred language

C#: appsettings.json
Python: .env
Update the configuration values to include the endpoint and a key from the Azure Language resource you created (available on the Keys and Endpoint page for your Azure AI Language resource in the Azure portal).

Save the configuration file.*/

// Import namespaces
using Azure;
using Azure.AI.Language.Conversations;

namespace clock_client
{
    class Program
    {

        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                //Guid lsAppId = Guid.Parse(configuration["LSAppID"]);
                string predictionEndpoint = configuration["AIServicesEndpoint"];
                string predictionKey = configuration["AIServicesKey"];

                // Create a client for the Language service model
                Uri endpoint = new Uri(predictionEndpoint);
                AzureKeyCredential credential = new AzureKeyCredential(predictionKey);

                ConversationAnalysisClient client = new ConversationAnalysisClient(endpoint, credential);
                // Get user input (until they enter "quit")
                string userText = "";
                while (userText.ToLower() != "quit")
                {
                    Console.WriteLine("\nEnter some text ('quit' to stop)");
                    userText = Console.ReadLine();
                    if (userText.ToLower() != "quit")
                    {

                        // Call the Language service model to get intent and entities
                        var projectName = "Clock";
                        var deploymentName = "production";
                        var data = new
                        {
                            analysisInput = new
                            {
                                conversationItem = new
                                {
                                    text = userText,
                                    id = "1",
                                    participantId = "1",
                                }
                            },
                            parameters = new
                            {
                                projectName,
                                deploymentName,
                                // Use Utf16CodeUnit for strings in .NET.
                                stringIndexType = "Utf16CodeUnit",
                            },
                            kind = "Conversation",
                        };
                        // Send request
                        Response response = await client.AnalyzeConversationAsync(RequestContent.Create(data));
                        dynamic conversationalTaskResult = response.Content.ToDynamicFromJson(JsonPropertyNames.CamelCase);
                        dynamic conversationPrediction = conversationalTaskResult.Result.Prediction;
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        Console.WriteLine(JsonSerializer.Serialize(conversationalTaskResult, options));
                        Console.WriteLine("--------------------\n");
                        Console.WriteLine(userText);
                        var topIntent = "";
                        if (conversationPrediction.Intents[0].ConfidenceScore > 0.5)
                        {
                            topIntent = conversationPrediction.TopIntent;
                        }
                        // Apply the appropriate action
                        switch (topIntent)
                        {
                            case "GetTime":
                                var location = "local";
                                // Check for a location entity
                                foreach (dynamic entity in conversationPrediction.Entities)
                                {
                                    if (entity.Category == "Location")
                                    {
                                        //Console.WriteLine($"Location Confidence: {entity.ConfidenceScore}");
                                        location = entity.Text;
                                    }
                                }
                                // Get the time for the specified location
                                string timeResponse = GetTime(location);
                                Console.WriteLine(timeResponse);
                                break;
                            case "GetDay":
                                var date = DateTime.Today.ToShortDateString();
                                // Check for a Date entity
                                foreach (dynamic entity in conversationPrediction.Entities)
                                {
                                    if (entity.Category == "Date")
                                    {
                                        //Console.WriteLine($"Location Confidence: {entity.ConfidenceScore}");
                                        date = entity.Text;
                                    }
                                }
                                // Get the day for the specified date
                                string dayResponse = GetDay(date);
                                Console.WriteLine(dayResponse);
                                break;
                            case "GetDate":
                                var day = DateTime.Today.DayOfWeek.ToString();
                                // Check for entities            
                                // Check for a Weekday entity
                                foreach (dynamic entity in conversationPrediction.Entities)
                                {
                                    if (entity.Category == "Weekday")
                                    {
                                        //Console.WriteLine($"Location Confidence: {entity.ConfidenceScore}");
                                        day = entity.Text;
                                    }
                                }
                                // Get the date for the specified day
                                string dateResponse = GetDate(day);
                                Console.WriteLine(dateResponse);
                                break;
                            default:
                                // Some other intent (for example, "None") was predicted
                                Console.WriteLine("Try asking me for the time, the day, or the date.");
                                break;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string GetTime(string location)
        {
            var timeString = "";
            var time = DateTime.Now;

            /* Note: To keep things simple, we'll ignore daylight savings time and support only a few cities.
               In a real app, you'd likely use a web service API (or write  more complex code!)
               Hopefully this simplified example is enough to get the the idea that you
               use LU to determine the intent and entities, then implement the appropriate logic */

            switch (location.ToLower())
            {
                case "local":
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "london":
                    time = DateTime.UtcNow;
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "sydney":
                    time = DateTime.UtcNow.AddHours(11);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "new york":
                    time = DateTime.UtcNow.AddHours(-5);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "nairobi":
                    time = DateTime.UtcNow.AddHours(3);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "tokyo":
                    time = DateTime.UtcNow.AddHours(9);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "delhi":
                    time = DateTime.UtcNow.AddHours(5.5);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                default:
                    timeString = "I don't know what time it is in " + location;
                    break;
            }

            return timeString;
        }

        static string GetDate(string day)
        {
            string date_string = "I can only determine dates for today or named days of the week.";

            // To keep things simple, assume the named day is in the current week (Sunday to Saturday)
            DayOfWeek weekDay;
            if (Enum.TryParse(day, true, out weekDay))
            {
                int weekDayNum = (int)weekDay;
                int todayNum = (int)DateTime.Today.DayOfWeek;
                int offset = weekDayNum - todayNum;
                date_string = DateTime.Today.AddDays(offset).ToShortDateString();
            }
            return date_string;

        }

        static string GetDay(string date)
        {
            // Note: To keep things simple, dates must be entered in US format (MM/DD/YYYY)
            string day_string = "Enter a date in MM/DD/YYYY format.";
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
            {
                day_string = dateTime.DayOfWeek.ToString();
            }

            return day_string;
        }
    }
}
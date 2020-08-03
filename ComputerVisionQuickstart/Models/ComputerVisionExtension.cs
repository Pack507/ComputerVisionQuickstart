using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ComputerVisionQuickstart.Models
{
    public class ComputerVisionExtension
    {

        // Add your Computer Vision subscription key and endpoint to your environment variables. 
        // Close/reopen your project for them to take effect.
        //static string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        //static string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");

        // requires using Microsoft.Extensions.Configuration;
        private readonly IConfiguration _configuration;
        private readonly string _key;
        private readonly string _endpoint;

        public ComputerVisionExtension(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = _configuration["COMPUTER_VISION:COMPUTER_VISION_SUBSCRIPTION_KEY"];
            _endpoint = _configuration["COMPUTER_VISION:COMPUTER_VISION_ENDPOINT"];
        }

        /*
        * AUTHENTICATE
        * Creates a Computer Vision client used by each example.
        */
        public ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        /* 
        * ANALYZE IMAGE - URL IMAGE
        * Analyze URL image. Extracts captions, categories, tags, objects, faces, racy/adult content,
        * brands, celebrities, landmarks, color scheme, and image types.
        */
        public async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
              VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
              VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
              VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
              VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
              VisualFeatureTypes.Objects
            };
        }

        /*
        * BATCH READ FILE - URL IMAGE
        * Recognizes handwritten text. 
        * This API call offers an improvement of results over the Recognize Text calls.
        */
        public async Task<string> BatchReadFileUrl(ComputerVisionClient client, string urlImage)
        {
            try
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine("BATCH READ FILE - URL IMAGE");
                Console.WriteLine();

                // Read text from URL
                var textHeaders = await client.BatchReadFileAsync(urlImage);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders.OperationLocation;

                return operationLocation;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.InnerException);
                throw;
            }
        }

        /*
        * BATCH READ FILE - STREAM IMAGE
        * Recognizes handwritten text. 
        * This API call offers an improvement of results over the Recognize Text calls.
        */
        public async Task<string> BatchReadFileStream(ComputerVisionClient client, Stream streamImage)
        {
            try
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine("BATCH READ FILE - STREAM IMAGE");
                Console.WriteLine();

                // Read text from URL
                var textHeaders = await client.BatchReadFileInStreamAsync(streamImage);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders.OperationLocation;

                return operationLocation;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.InnerException);
                throw;
            }
        }

        private async Task<List<string>> GetTextAsync(ComputerVisionClient computerVision, string operationLocation)
        {
            string operationId = operationLocation.Substring(
                operationLocation.Length - 36);

            ReadOperationResult result = await computerVision.GetReadOperationResultAsync(operationId);

            // Wait for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                Console.WriteLine(
                    "Server status: {0}, waiting {1} seconds...", result.Status, i);
                await Task.Delay(1000);

                result = await computerVision.GetReadOperationResultAsync(operationId);
            }

            // Display the results
            Debug.WriteLine("");
            var recResults = result.RecognitionResults;
            List<string> listText = new List<string>();

            foreach (TextRecognitionResult recResult in recResults)
            {
                foreach (Line line in recResult.Lines)
                {
                    Debug.WriteLine("" + line.Text);
                    listText.Add(line.Text);
                }
            }
            Debug.WriteLine("");

            return listText;
        }


        public async Task<List<string>> AnalyzeFileByUrl(string urlImage)
        {
            ComputerVisionClient client = Authenticate(_endpoint, _key);

            string resultOperationLocation = await BatchReadFileUrl(client, urlImage);

            List<string> listTextResult = await GetTextAsync(client, resultOperationLocation);

            return listTextResult;
        }

        public async Task<List<string>> AnalyzeFileByStream(Stream streamImage)
        {
            ComputerVisionClient client = Authenticate(_endpoint, _key);

            string resultOperationLocation = await BatchReadFileStream(client, streamImage);

            List<string> listTextResult = await GetTextAsync(client, resultOperationLocation);

            return listTextResult;
        }
    }
}

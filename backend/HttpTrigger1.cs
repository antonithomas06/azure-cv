using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;


namespace Company.Function
{
    public class HttpTrigger1
    {
        private readonly IConfiguration _config;

        public HttpTrigger1(IConfiguration config)
        {
            _config = config;
        }
        [FunctionName("HttpTrigger1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string cosmosDbEndpoint = _config["CosmosDbEndpoint"];
            string cosmosDbKey = _config["CosmosDbKey"];
            string cosmosDbDatabaseName = _config["AzureDB"];
            string cosmosDbContainerName = _config["AzureContainer"];
            string AzureConnectionString = _config["AzureConnectionString"];


            // Create a new instance of the CosmosClient
            CosmosClient cosmosClient = new CosmosClient(AzureConnectionString);
            // Get a reference to the container
            Container container = cosmosClient.GetContainer(cosmosDbDatabaseName, cosmosDbContainerName);

            // Get a reference to the item
            ItemResponse<dynamic> itemResponse = await container.ReadItemAsync<dynamic>("1", new PartitionKey("1"));

            // Get the value from the item
            dynamic value = itemResponse.Resource;
            int count = value.counter;
            

            count++;


            dynamic updateValue = new { id = "1", counter = count, partitionKey = "1" };

            await container.ReplaceItemAsync(updateValue, "1", new PartitionKey("1"));

            string json1 = JsonConvert.SerializeObject(updateValue);
            
            
            

            string responseMessage = string.IsNullOrEmpty(name)
               ? $"{updateValue}"
                : $"Hello, {name} {value}";

            return new OkObjectResult(itemResponse.Resource);
        }
    }
}
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

var connStr = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
if(string.IsNullOrEmpty(connStr))
    connStr= "UseDevelopmentStorage=true";
Console.WriteLine($"will use the connection string {connStr} ");
if (!PromptConfirmation("Are you sure you want to clean all Tables and Queues ?"))
{
    return;
}

var TserviceClient = new TableServiceClient(connStr);

Pageable<TableItem> queryTableResults = TserviceClient.Query();
foreach (var table in queryTableResults)
{
    if (table.Name.StartsWith("flow"))
    {
        TserviceClient.DeleteTable(table.Name);
        Console.WriteLine($"-Table {table.Name} is Deleted ");
    }

}


var QserviceClient = new QueueServiceClient(connStr);

//list all queues in the storage account
var myqueues = QserviceClient.GetQueues().AsPages();

//then you can write code to list all the queue names          
foreach (Azure.Page<QueueItem> queuePage in myqueues)
{
    foreach (var queue in queuePage.Values)
    {
        if (queue.Name.StartsWith("flow"))
        {
            QserviceClient.DeleteQueue(queue.Name);
            Console.WriteLine($"-Queue {queue.Name} is Deleted ");
        }


    }

}

Console.WriteLine("Done");
return;


static bool PromptConfirmation(string confirmText)
{
    Console.Write(confirmText + " [y/n] : ");
    var response = Console.ReadLine();
    Console.WriteLine();
    return (response?.ToLower() =="y" );
}
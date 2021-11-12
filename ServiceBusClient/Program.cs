using Microsoft.Extensions.Configuration;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ExecuteAsync().GetAwaiter().GetResult();
        }

        public static async Task ExecuteAsync()
        {
            InitializeConfiguration();
            var topicPath = Configuration["ServiceBusTopicPath"];
            var subscriptionName = Configuration["ServiceBusSubscription"];
            var connectionString = Configuration["ServiceBusConnectionString"];

            try
            {
                // Create a namespace client
                var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);            // Create a queue client
                                                                                                                 // Create a subscription client 
                var subscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, topicPath, subscriptionName);
                // Peek a queue
                var message = await subscriptionClient.PeekAsync();
                // Get the queue metadata with namespace
                var subscriptionDesc = await namespaceManager.GetSubscriptionAsync(topicPath, subscriptionName);
                Console.WriteLine("Works Fine");
                Console.ReadLine();
            }
            catch (UnauthorizedAccessException e)
            {
                // This exception is expected. 
                var errormsg = string.Format(
                           "Connection string does not have Manage claim for topic '{0}', subscription '{1}'. Failed to get subscription description to derive topic queue length metric. Falling back to using first message enqueued time.",
                           topicPath,
                           subscriptionName);
                Console.WriteLine(errormsg);
                Console.ReadLine();
            }
        }

        private static IConfigurationRoot Configuration { get; set; }

        private static void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

    }
}

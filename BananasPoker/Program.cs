using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplesGrainInterfaces;
using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Streams;
using Orleans.Streams.Core;
using Orleans.Streams.PubSub;

namespace BananasPoker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ClientConfiguration clientConfig = ClientConfiguration.LocalhostSilo();
            clientConfig.AddSimpleMessageStreamProvider("ApplesStreamProvider");
            IClusterClient client = new ClientBuilder().UseConfiguration(clientConfig).Build();

            await client.Connect();

            Guid appleKey = Guid.NewGuid();

            IAppleGrain myAppleGrain = client.GetGrain<IAppleGrain>(appleKey);

            IStreamProvider applesStreamProvider = client.GetStreamProvider("ApplesStreamProvider");

            IAsyncStream<string> applesStream = applesStreamProvider.GetStream<string>(appleKey, "ApplesStream");

            while (true)
            {
                Console.WriteLine("Enter 'Say(A)pple' to have a grain say Apple");
                Console.WriteLine("Enter 'Send(S)treamMessage' to send a stream message");
                Console.WriteLine("Enter '(C)omplete' to complete the stream");
                Console.WriteLine("Enter '(R)ekSubscriptions' to monkey with subscription manager");
                Console.WriteLine("Enter 'E(x)it' to stop");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "SayApple":
                    case "A":
                    case "a":
                        await myAppleGrain.SayApple();
                        continue;
                    case "SendStreamMessage":
                    case "S":
                    case "s":
                        Console.Write("\nEnter message to send:");
                        string messageToSend = Console.ReadLine();
                        await applesStream.OnNextAsync(messageToSend);
                        continue;
                    case "Complete":
                    case "C":
                    case "c":
                        await applesStream.OnCompletedAsync();
                        continue;
                    case "RekSubscriptions":
                    case "R":
                    case "r":
                        applesStreamProvider.TryGetStreamSubscrptionManager(out var manager);
                        IStreamIdentity applesStreamIdentity = new StreamIdentity(appleKey, "ApplesStream");
                        var applesStreamSubscriptions = await manager.GetSubscriptions("ApplesStreamProvider", applesStreamIdentity);
                        foreach (var subscription in applesStreamSubscriptions)
                        {
                            await manager.RemoveSubscription(subscription.StreamProviderName, subscription.StreamId, subscription.SubscriptionId);
                        }
                        var applesStreamSubscriptionsAfterRemoval = await manager.GetSubscriptions("ApplesStreamProvider", applesStreamIdentity);
                        continue;
                    case "Exit":
                    case "X":
                    case "x":
                        break;
                    default:
                        Console.WriteLine("Bad option");
                        continue;
                }

                break;
            }

            //We're done now, so let's mark the stream completed, and close out our client
            await applesStream.OnCompletedAsync();
            
            await client.Close();
        }
    }
}

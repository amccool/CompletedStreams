using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplesGrainInterfaces;
using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Streams;

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
                Console.WriteLine("Enter 'E(x)it' to stop");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "SayApple":
                    case "A":
                        await myAppleGrain.SayApple();
                        break;
                    case "SendStreamMessage":
                    case "S":
                        Console.Write("\nEnter message to send:");
                        string messageToSend = Console.ReadLine();
                        await applesStream.OnNextAsync(messageToSend);
                        break;
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
        }
    }
}

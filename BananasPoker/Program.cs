using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime.Configuration;

namespace BananasPoker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ClientConfiguration clientConfig = ClientConfiguration.LocalhostSilo();

            IClusterClient client = new ClientBuilder().UseConfiguration(clientConfig).Build();

            await client.Connect();

            while (true)
            {

            }
        }
    }
}

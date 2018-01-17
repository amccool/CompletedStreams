using System;

using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace DebugSiloHost
{
    /// <summary>
    /// Orleans test silo host
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // First, configure and start a local silo
            var siloConfig = ClusterConfiguration.LocalhostPrimarySilo();
            siloConfig.AddSimpleMessageStreamProvider("ApplesStreamProvider");
            var silo = new SiloHost("TestSilo", siloConfig);
            silo.InitializeOrleansSilo();
            silo.StartOrleansSilo();

            Console.WriteLine("Silo started.");
            

            //
            // This is the place for your test code.
            //

            Console.WriteLine("\nPress Enter to terminate...");
            Console.ReadLine();

            // Shut down
            silo.ShutdownOrleansSilo();
        }
    }
}

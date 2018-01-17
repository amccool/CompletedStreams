using System;
using System.Threading.Tasks;
using ApplesGrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace ApplesGrains
{
    /// <summary>
    /// Grain implementation class AppleGrain.
    /// </summary>
    public class AppleGrain : Grain, IAppleGrain
    {
        private StreamSubscriptionHandle<string> _applesStreamHandle;

        private string _myFakeConfig;
        
        public Task SayApple()
        {
            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- {DateTime.UtcNow}: Apple!");

            return Task.CompletedTask;
        }

        public async Task SubscribeToAppleStream(string appleStreamToSubscribeTo)
        {
            IStreamProvider applesStreamProvider = this.GetStreamProvider("ApplesStreamProvider");

            IAsyncStream<string> applesStream = applesStreamProvider.GetStream<string>(this.GetPrimaryKey(), appleStreamToSubscribeTo);

            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- Subscribing to stream {this.GetPrimaryKey()}-{appleStreamToSubscribeTo}");

            _applesStreamHandle = await applesStream.SubscribeAsync(async (x, y) =>
            {
                Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- {DateTime.UtcNow}: {x} --- config: {_myFakeConfig}");
                await Task.Delay(TimeSpan.FromSeconds(1));
            },
                async error => Console.WriteLine($"Error: {error.StackTrace}"),
                async () => await StreamCompleted());

            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- Subscribed to stream {this.GetPrimaryKey()}-{appleStreamToSubscribeTo}");
        }

        public Task LoadFakeConfig(string fakeConfig)
        {
            _myFakeConfig = fakeConfig;

            return Task.CompletedTask;
        }


        private async Task StreamCompleted()
        {
            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- My stream completed!");

            await _applesStreamHandle.UnsubscribeAsync();

            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- Unsubscribed from stream");
        }
    }
}

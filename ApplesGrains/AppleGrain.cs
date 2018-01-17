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
    [ImplicitStreamSubscription("ApplesStream")]
    public class AppleGrain : Grain, IAppleGrain
    {
        private StreamSubscriptionHandle<string> _applesStreamHandle;

        public override async Task OnActivateAsync()
        {
            await SubscribeToAppleStream();
        }

        public Task SayApple()
        {
            Console.WriteLine($"{DateTime.UtcNow}: Apple!");

            return Task.CompletedTask;
        }

        public async Task SubscribeToAppleStream()
        {
            IStreamProvider applesStreamProvider = this.GetStreamProvider("ApplesStreamProvider");

            IAsyncStream<string> applesStream = applesStreamProvider.GetStream<string>(this.GetPrimaryKey(), "ApplesStream");

            Console.WriteLine($"Subscribing to stream {this.GetPrimaryKey()}-ApplesStream");

            _applesStreamHandle = await applesStream.SubscribeAsync(async (x, y) => Console.WriteLine(x),
                async error => Console.WriteLine($"Error: {error.StackTrace}"),
                async () => await StreamCompleted());
        }

        private async Task StreamCompleted()
        {
            Console.WriteLine($"My stream completed!");

            await _applesStreamHandle.UnsubscribeAsync();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
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
        private List<StreamSubscriptionHandle<string>> _myHandles = new List<StreamSubscriptionHandle<string>>();

        public override async Task OnActivateAsync()
        {
            IStreamProvider applesStreamProvider = this.GetStreamProvider("ApplesStreamProvider");

            var applesStream = applesStreamProvider.GetStream<string>(this.GetPrimaryKey(), "ApplesStream");

            var handles = await applesStream.GetAllSubscriptionHandles();
            
            foreach (var handle in handles)
            {
                _myHandles.Add(await handle.ResumeAsync(async (x, y) => Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- {DateTime.UtcNow}: {x}"),
                    async error => Console.WriteLine($"Error: {error.StackTrace}"),
                    async () => await StreamCompleted()));
            }
        }

        public Task SayApple()
        {
            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- {DateTime.UtcNow}: Apple!");

            return Task.CompletedTask;
        }

        public async Task SubscribeToAppleStream()
        {
            IStreamProvider applesStreamProvider = this.GetStreamProvider("ApplesStreamProvider");

            IAsyncStream<string> applesStream = applesStreamProvider.GetStream<string>(this.GetPrimaryKey(), "ApplesStream");

            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- Subscribing to stream {this.GetPrimaryKey()}-ApplesStream");

            _myHandles.Add(await applesStream.SubscribeAsync(async (x, y) => Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- {DateTime.UtcNow}: {x}"),
                async error => Console.WriteLine($"Error: {error.StackTrace}"),
                async () => await StreamCompleted()));

            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- Subscribed to stream {this.GetPrimaryKey()}-ApplesStream");
        }

        private async Task StreamCompleted()
        {
            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- My stream completed!");

            foreach (var handle in _myHandles)
            {
                await handle.UnsubscribeAsync();
            }

            Console.WriteLine($"{nameof(AppleGrain)}-{this.GetPrimaryKey()} --- Unsubscribed from stream");
        }

        public Task MarkYourselfForDeactivation()
        {
            DeactivateOnIdle();

            return Task.CompletedTask;
        }
    }
}

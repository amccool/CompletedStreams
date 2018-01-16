using System.Threading.Tasks;
using ApplesGrainInterfaces;
using Orleans;

namespace ApplesGrains
{
    /// <summary>
    /// Grain implementation class AppleGrain.
    /// </summary>
    public class AppleGrain : Grain, IAppleGrain
    {
        public Task SayApple()
        {
            throw new System.NotImplementedException();
        }

        public Task SubscribeToAppleStream()
        {
            throw new System.NotImplementedException();
        }
    }
}

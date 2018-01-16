using System.Threading.Tasks;
using Orleans;

namespace ApplesGrainInterfaces
{
    /// <summary>
    /// Grain interface IAppleGrain
    /// </summary>
    public interface IAppleGrain : IGrainWithGuidKey
    {
        Task SayApple();

        Task SubscribeToAppleStream();
    }
}

using System;
using System.Threading.Tasks;

namespace EvHttpSharp
{
    public interface IEventHttpListener : IDisposable
    {
        void Start(string host, ushort port);
        void Start(IntPtr sharedSocket);
        Task StopListeningAsync();
        Task WaitForPendingConnections();
        Task Shutdown();
    }
}
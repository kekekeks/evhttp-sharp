using EvHttpSharp.Interop;

namespace EvHttpSharp
{
    public static class LibLocator
    {
		public static void Init(string basePath = null)
		{
			Event.Init(basePath);
		}
    }
}

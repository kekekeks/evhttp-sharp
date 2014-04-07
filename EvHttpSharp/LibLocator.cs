using System;
using EvHttpSharp.Interop;

namespace EvHttpSharp
{
    public static class LibLocator
    {
        private static bool _initialized;
        public static void Init(string basePath = null)
        {
            if (_initialized)
                throw new InvalidOperationException("Already initialized");
            Event.Init(basePath);
            _initialized = true;
        }

        internal static void TryToLoadDefaultIfNotInitialized()
        {
            if(_initialized)
                return;
            Init();
        }
    }
}

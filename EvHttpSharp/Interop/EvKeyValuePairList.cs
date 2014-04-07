using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    struct EvKeyValuePair
    {
        public readonly IntPtr Next;
        private readonly IntPtr _ignored;
        public readonly IntPtr Key;
        public readonly IntPtr Value;


    
        public static IDictionary<string, IEnumerable<string>> ExtractDictinary(IntPtr ptr)
        {
            var dic = new Dictionary<string, List<string>>();

            //Extract pointer to the first element
            ptr = Marshal.ReadIntPtr(ptr);

            while (ptr != IntPtr.Zero)
            {
                var data = (EvKeyValuePair) Marshal.PtrToStructure(ptr, typeof (EvKeyValuePair));
                ptr = data.Next;
                var key = Marshal.PtrToStringAnsi(data.Key);
                var value = Marshal.PtrToStringAnsi(data.Value);
                if (key == null)
                    continue;
                List<string> l;
                if (!dic.TryGetValue(key, out l))
                    dic[key] = l = new List<string>();
                l.Add(value);
                
            }

            return dic.ToDictionary(p => p.Key, p => (IEnumerable<string>) p.Value);
        }


    }
}

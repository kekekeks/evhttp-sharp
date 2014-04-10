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


    
        public static unsafe IDictionary<string, IEnumerable<string>> ExtractDictinary(IntPtr ptr)
        {
            var dic = new Dictionary<string, IEnumerable<string>>();

            //Extract pointer to the first element
            ptr = Marshal.ReadIntPtr(ptr);

            while (ptr != IntPtr.Zero)
            {
                var data = (EvKeyValuePair*)ptr;
                ptr = data->Next;
                var key = Marshal.PtrToStringAnsi(data->Key);
                var value = Marshal.PtrToStringAnsi(data->Value);
                if (key == null)
                    continue;
                IEnumerable<string> l;
                if (!dic.TryGetValue(key, out l))
                    dic[key] = l = new[] {value};
                else
                {
                    var lst = l as List<string>;
                    if (lst != null)
                        ((List<string>) l).Add(value);
                    else
                        dic[key] = new List<string> {((string[]) l)[0], value};
                }
            }

            return dic;
        }


    }
}

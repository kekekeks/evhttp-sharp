using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
    static class Event
    {
        // ReSharper disable InconsistentNaming    
        [StructLayout(LayoutKind.Sequential)]
        internal struct sockaddr_in
        {
            internal short sin_family;
            internal ushort sin_port;
            internal int sin_addr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            internal byte[] sin_zero;
        }

        public const int AF_INET = 2;

        public static class D
        {
            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void event_base_free(IntPtr eventBase);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate EventBase event_base_new();

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate EvHttp evhttp_new(EventBase eventBase);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void evhttp_free(IntPtr evhttp);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void thread_model_selector();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void log_cb(int sev, IntPtr message);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void event_set_log_callback(log_cb cb);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void evhttp_request_callback(IntPtr request, IntPtr arg);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void evhttp_set_gencb(EvHttp http, evhttp_request_callback cb, IntPtr arg);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate EvHttpBoundSocket evhttp_bind_socket_with_handle(
                EvHttp http, [MarshalAs(UnmanagedType.LPStr)] string host, ushort port);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void evhttp_del_accept_socket(EvHttp evhttp, EvHttpBoundSocket socket);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void event_base_dispatch(EventBase eventBase);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void event_base_loopbreak(EventBase eventBase);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void evhttp_send_reply(
                EvHttpRequest req, int code, [MarshalAs(UnmanagedType.LPStr)] string desc, EvBuffer buffer);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void evhttp_set_allowed_methods(EvHttp http, EvHttpCmdType methods);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate EvHttpCmdType evhttp_request_get_command(EvHttpRequest request);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate EvHttpConnection evhttp_request_get_connection (EvHttpRequest request);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate IntPtr evhttp_request_get_uri(EvHttpRequest request);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate IntPtr evhttp_request_get_host(EvHttpRequest request);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate IntPtr evhttp_request_get_input_headers(EvHttpRequest request);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate IntPtr evhttp_request_get_input_buffer(EvHttpRequest request);


            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void evhttp_connection_get_peer(EvHttpConnection connection, ref IntPtr pHostString, ref ushort port);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate EvBuffer evbuffer_new();

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void evbuffer_free (IntPtr handle);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate IntPtr evbuffer_get_length(EvBuffer buffer);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate int evbuffer_remove(EvBuffer evBuffer, byte[] buffer, IntPtr len);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate int evbuffer_add (EvBuffer evBuffer, byte[] buffer, IntPtr len);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate IntPtr evhttp_request_get_output_headers (EvHttpRequest request);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void evhttp_add_header(
                IntPtr headers, [MarshalAs(UnmanagedType.LPStr)] string header, [MarshalAs(UnmanagedType.LPStr)] string value);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate int event_base_loop(EventBase eventBase, EvLoopFlags flags);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate int event_free (IntPtr ev);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void event_active (IntPtr ev, int res = 0, short ignored = 0);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void evconnlistener_free(IntPtr handle);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate EvHttpBoundSocket evhttp_bind_listener(EvHttp http, EvConnListener listener);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate EvConnListener evconnlistener_new_bind(
                EventBase evBase, IntPtr cb, IntPtr ptr, uint flags, int backlog, ref sockaddr_in sockaddr, int socklen);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int ev_add_virtual_host(
                    EvHttp http, string pattern, EvHttp host);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int ev_add_server_alias(
                    EvHttp http, string pattern);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int ev_remove_server_alias(
                    EvHttp http, string pattern);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int ev_remove_virtual_host(
                    EvHttp http, EvHttp host);
#region *nix
            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void event_callback_normal(int fd, short events, IntPtr arg);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate EvEvent event_new_normal (EventBase eventBase, int fd, short events, event_callback_normal cb, IntPtr arg);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int evconnlistener_get_fd_normal(EvConnListener l);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate EvHttpBoundSocket evhttp_accept_socket_with_handle_normal(EvHttp http, int fd);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate EvConnListener evconnlistener_new_normal(
            EventBase evBase, IntPtr cb, IntPtr ptr, uint flags, int backlog, int fd);

#endregion
#region Windows
            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate void event_callback_windows(IntPtr fd, short events, IntPtr arg);

            [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            public delegate EvEvent event_new_windows(EventBase eventBase, IntPtr fd, short events, event_callback_windows cb, IntPtr arg);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate IntPtr evconnlistener_get_fd_windows(EvConnListener l);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate EvHttpBoundSocket evhttp_accept_socket_with_handle_windows(EvHttp http, IntPtr fd);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate EvConnListener evconnlistener_new_windows(
                EventBase evBase, IntPtr cb, IntPtr ptr, uint flags, int backlog, IntPtr fd);

#endregion

        }

        // ReSharper restore InconsistentNaming


        [EvImport] public static D.event_set_log_callback EventSetLogCallback;
        [EvImport] public static D.event_base_free EventBaseFree;
        [EvImport] public static D.event_base_new EventBaseNew;
        [EvImport] public static D.event_base_dispatch EventBaseDispatch;
        [EvImport] public static D.event_base_loopbreak EventBaseLoopbreak;
        [EvImport] public static D.evbuffer_free EvBufferFree;
        [EvImport] public static D.evbuffer_new EvBufferNew;
        [EvImport] public static D.evbuffer_get_length EvBufferGetLength;
        [EvImport] public static D.evbuffer_remove EvBufferRemove;
        [EvImport] public static D.evbuffer_add EvBufferAdd;
        [EvImport] public static D.event_base_loop EventBaseLoop;
        [EvImport] public static D.event_free EventFree;
        [EvImport] public static D.event_active EventActive;
        [EvImport] public static D.evconnlistener_free EvConnListenerFree;
        [EvImport] public static D.evconnlistener_new_bind EvConnListenerNewBind;
        [EvImport(EvDll.Core, "event_new")] public static D.event_new_windows EventNewWindows;
        [EvImport(EvDll.Core, "event_new")] public static D.event_new_normal EventNewNix;
        [EvImport (EvDll.Core, "evconnlistener_get_fd")] public static D.evconnlistener_get_fd_windows EvConnListenerGetFdWindows;
        [EvImport(EvDll.Core, "evconnlistener_get_fd")] public static D.evconnlistener_get_fd_normal EvConnListenerGetFdNix;
        [EvImport (EvDll.Core, "evconnlistener_new")] public static D.evconnlistener_new_windows EvConnListenerNewWindows;
        [EvImport(EvDll.Core, "evconnlistener_new")] public static D.evconnlistener_new_normal EvConnListenerNewNix;
        [EvImport(EvDll.Extra, "evhttp_accept_socket_with_handle")] public static D.evhttp_accept_socket_with_handle_windows EvHttpAcceptSocketWithHandleWindows;
        [EvImport(EvDll.Extra, "evhttp_accept_socket_with_handle")] public static D.evhttp_accept_socket_with_handle_normal EvHttpAcceptSocketWithHandleNix;
        [EvImport(EvDll.Extra)] public static D.evhttp_bind_listener EvHttpBindListener;
        [EvImport(EvDll.Extra)] public static D.evhttp_free EvHttpFree;
        [EvImport(EvDll.Extra)] public static D.evhttp_new EvHttpNew;
        [EvImport(EvDll.Extra)] public static D.evhttp_set_gencb EvHttpSetGenCb;
        [EvImport(EvDll.Extra)] public static D.evhttp_bind_socket_with_handle EvHttpBindSocketWithHandle;
        [EvImport(EvDll.Extra)] public static D.evhttp_del_accept_socket EvHttpDelAcceptSocket;
        [EvImport(EvDll.Extra)] public static D.evhttp_send_reply EvHttpSendReply;
        [EvImport(EvDll.Extra)] public static D.evhttp_set_allowed_methods EvHttpSetAllowedMethods;
        [EvImport(EvDll.Extra)] public static D.evhttp_request_get_command EvHttpRequestGetCommand;
        [EvImport(EvDll.Extra)] public static D.evhttp_request_get_connection EvHttpRequestGetConnection;
        [EvImport(EvDll.Extra)] public static D.evhttp_request_get_uri EvHttpRequestGetUri;
        [EvImport(EvDll.Extra)] public static D.evhttp_request_get_host EvHttpRequestGetHost;
        [EvImport(EvDll.Extra)] public static D.evhttp_request_get_input_headers EvHttpRequestGetInputHeaders;
        [EvImport(EvDll.Extra)] public static D.evhttp_request_get_input_buffer EvHttpRequestGetInputBuffer;
        [EvImport(EvDll.Extra)] public static D.evhttp_request_get_output_headers EvHttpRequestGetOutputHeaders;
        [EvImport(EvDll.Extra)] public static D.evhttp_connection_get_peer EvHttpConnectionGetPeer;
        [EvImport(EvDll.Extra)] public static D.evhttp_add_header EvHttpAddHeader;


        public static EvHttpBoundSocket EvHttpAcceptSocketWithHandle(EvHttp http, IntPtr fd)
        {
            return Platform.RunningOnWindows ? EvHttpAcceptSocketWithHandleWindows(http, fd) : EvHttpAcceptSocketWithHandleNix(http, fd.ToInt32());
        }

        public static EvConnListener EvConnListenerNew(EventBase evBase, IntPtr cb, IntPtr ptr, uint flags, int backlog, IntPtr fd)
        {
            return Platform.RunningOnWindows
                       ? EvConnListenerNewWindows(evBase, cb, ptr, flags, backlog, fd)

                       : EvConnListenerNewNix(evBase, cb, ptr, flags, backlog, fd.ToInt32());

        }

        
    
        public static event Action<int, string> Log;

        private static readonly D.log_cb LogCallbackDelegate = LogCallback;
        static void LogCallback(int sev, IntPtr message)
        {
            var msg = Marshal.PtrToStringAnsi(message);
            if (Log != null)
                Log(sev, msg);
            else
                Console.WriteLine(msg);
        }

        [DllImport("Ws2_32.dll")]
        static extern int WSAStartup(ushort wVersionRequested, byte[] data);

        public static void Init (string basePath = null)
        {
            var platform = Environment.OSVersion.Platform;

            if (platform == PlatformID.Unix && File.Exists("/usr/lib/libSystem.dylib"))
                platform = PlatformID.MacOSX;

            IDynLoader loader;
            switch (platform)
            {
                case PlatformID.Win32NT:
                    WSAStartup(514, new byte[512]);
                    loader = new Win32Loader();
                    break;
                case PlatformID.Unix:
                    loader = new LinuxLoader();
                    break;
                case PlatformID.MacOSX:
                    loader = new OsxLoader();
                    break;
                default:
                    throw new NotSupportedException("Platform " + platform + " is not supported.");
            }

            var core = loader.LoadLibrary(basePath, "libevent_core");
            var extra = loader.LoadLibrary(basePath, "libevent_extra");
            foreach (var fieldInfo in typeof (Event).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (!typeof (Delegate).IsAssignableFrom(fieldInfo.FieldType))
                    continue;
                var import = fieldInfo.GetCustomAttributes (typeof (EvImportAttribute), true).Cast<EvImportAttribute> ().First ();
                
                var lib = import.Dll == EvDll.Core ? core : extra;
                var funcPtr = loader.GetProcAddress(lib, import.Name ?? fieldInfo.FieldType.Name);
                fieldInfo.SetValue(null, Marshal.GetDelegateForFunctionPointer(funcPtr, fieldInfo.FieldType));
            }

            EventSetLogCallback(LogCallbackDelegate);
            var selectorPtr = platform == PlatformID.Win32NT
                                  ? loader.GetProcAddress(core, "evthread_use_windows_threads")
                                  : loader.GetProcAddress(loader.LoadLibrary(basePath, "libevent_pthreads"), "evthread_use_pthreads");
            ((D.thread_model_selector) Marshal.GetDelegateForFunctionPointer(selectorPtr, typeof (D.thread_model_selector)))();
        }


    }
}

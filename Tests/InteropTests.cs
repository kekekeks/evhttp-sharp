using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EvHttpSharp.Interop;
using Xunit;

namespace Tests
{
    public class InteropTests : TestBase
    {
        [Fact]
        public void CheckEventBase()
        {
            var b = Event.EventBaseNew();
            Assert.False(b.IsInvalid);
            Assert.NotEqual(IntPtr.Zero, b.DangerousGetHandle());
            b.Dispose();
        }

        [Fact]
        public void CheckEvHttp()
        {
            using (var b = Event.EventBaseNew())
            using (var http = Event.EvHttpNew(b))
            {
                Assert.False (http.IsInvalid);
                Assert.NotEqual (IntPtr.Zero, http.DangerousGetHandle ());
            }
        }
    }
}

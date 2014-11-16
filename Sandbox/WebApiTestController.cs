using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sandbox
{
    public class WebApiTestController : ApiController
    {
        [HttpGet, Route("test")]
        public object Test()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Hello world!", Encoding.UTF8)
            };
        }
    }
}

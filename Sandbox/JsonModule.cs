using Nancy;

namespace Sandbox
{
    public class JsonModule : NancyModule
    {
        public JsonModule()
            : base("/json")
        {
            Get["/"] = _ => Response.AsJson(new {message = "Hello, World!"});
        }

    }
}

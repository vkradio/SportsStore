using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Infrastructure
{
    public class CustomJsonContractResolver : DefaultContractResolver
    {
        public CustomJsonContractResolver() => NamingStrategy = new CamelCaseNamingStrategy();
    }
}

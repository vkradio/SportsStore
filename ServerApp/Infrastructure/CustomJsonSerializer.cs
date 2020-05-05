using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Infrastructure
{
    public static class CustomJsonSerializer
    {
        public static string Serialize(object data) =>
            JsonConvert.SerializeObject(data, new JsonSerializerSettings { ContractResolver = new CustomJsonContractResolver() });
    }
}

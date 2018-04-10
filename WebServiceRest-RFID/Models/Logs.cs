using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceRest_RFID.Models
{
    public class Logs
    {
        [JsonProperty(PropertyName = "Logs")]
        public List<Log> logs { get; set; }
    }
}
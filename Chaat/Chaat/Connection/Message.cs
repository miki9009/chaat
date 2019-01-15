using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chaat.Connection
{
    public class Message
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [JsonProperty(PropertyName = "Body")]
        public string Body { get; set; }

        [JsonProperty(PropertyName = "PhotoLink")]
        public string PhotoLink { get; set; }

        [JsonProperty(PropertyName = "SenderID")]
        public int SenderID { get; set; }

        [JsonProperty(PropertyName = "RecieverID")]
        public int RecieverID { get; set; }

        [JsonProperty(PropertyName = "SocketTo")]
        public string SocketTo { get; set; }

        [JsonProperty(PropertyName = "SenderName")]
        public string SenderName { get; set; }
    }
}

using Chaat.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chaat.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string AspId { get; set; }
        public string Name { get; set; }
        public List <Message> Messages { get; set; }
    }
}

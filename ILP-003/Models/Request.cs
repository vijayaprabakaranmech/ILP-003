using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ILP_003.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public string Location { get; set; }
        public string Purpose { get; set; }
        public int Duration { get; set; }
        public string VMType { get; set; }
        public string VMSize { get; set; }
        public string IP { get; set; }
        public DateTime RequestedDateTime { get; set; }
    }
}
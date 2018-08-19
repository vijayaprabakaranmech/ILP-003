using ILP_003.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ILP_003.Data
{
    public class Data
    {
        public static List<Request> GetRequests()
        {
            List<Request> requests = new List<Request>
            {
                new Request{ UserName = "Vijayaprabakaran S", EmailID = "vijay@wipro.com",
                 Location = "Bengaluru", Purpose = "Training", Duration = 5,
                 VMType = ".NET SDK", VMSize = "Intel Xeon E5-2670 @ 2.6 GHZ", IP = "10.10.10.10", RequestedDateTime = DateTime.Now},
            };
            return requests;
        }
    }
}
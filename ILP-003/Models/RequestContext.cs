using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ILP_003.Models
{
    public class RequestContext : DbContext
    {
        public RequestContext()
            : base("MyDB")
        { }

        public DbSet<Request> requests { get; set; }
    }
}
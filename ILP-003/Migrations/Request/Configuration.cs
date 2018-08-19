namespace ILP_003.Migrations.Request
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ILP_003.Models.RequestContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\Request";
        }

        protected override void Seed(ILP_003.Models.RequestContext context)
        {
            context.requests.AddOrUpdate(
                r => new { r.EmailID }, Data.Data.GetRequests().ToArray());
        }
    }
}

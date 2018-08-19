namespace ILP_003.Migrations.Request
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "IP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "IP");
        }
    }
}

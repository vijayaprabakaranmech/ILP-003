namespace ILP_003.Migrations.Request
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        EmailID = c.String(),
                        Location = c.String(),
                        Purpose = c.String(),
                        Duration = c.Int(nullable: false),
                        VMType = c.String(),
                        VMSize = c.String(),
                        RequestedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Requests");
        }
    }
}

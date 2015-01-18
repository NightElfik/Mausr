namespace Mausr.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Drawings", "ClientGuid");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Drawings", new[] { "ClientGuid" });
        }
    }
}

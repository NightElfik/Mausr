namespace Mausr.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApprovedDrawings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Drawings", "LowQuality", c => c.Boolean(nullable: false));
            AddColumn("dbo.Drawings", "PromotedSymbolDrawing_SymbolDrawingId", c => c.Int());
            AddColumn("dbo.SymbolDrawings", "Approved", c => c.Boolean());
            CreateIndex("dbo.Drawings", "PromotedSymbolDrawing_SymbolDrawingId");
            AddForeignKey("dbo.Drawings", "PromotedSymbolDrawing_SymbolDrawingId", "dbo.SymbolDrawings", "SymbolDrawingId");
            DropColumn("dbo.Drawings", "Learned");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Drawings", "Learned", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Drawings", "PromotedSymbolDrawing_SymbolDrawingId", "dbo.SymbolDrawings");
            DropIndex("dbo.Drawings", new[] { "PromotedSymbolDrawing_SymbolDrawingId" });
            DropColumn("dbo.SymbolDrawings", "Approved");
            DropColumn("dbo.Drawings", "PromotedSymbolDrawing_SymbolDrawingId");
            DropColumn("dbo.Drawings", "LowQuality");
        }
    }
}

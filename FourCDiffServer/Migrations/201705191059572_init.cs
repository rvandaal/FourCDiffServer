namespace FourCDiffServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiffItems",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        LeftSide = c.String(),
                        RightSide = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DiffItems");
        }
    }
}

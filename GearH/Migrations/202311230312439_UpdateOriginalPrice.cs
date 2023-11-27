namespace GearH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOriginalPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "OriginalPrice", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "OriginalPrice");
        }
    }
}

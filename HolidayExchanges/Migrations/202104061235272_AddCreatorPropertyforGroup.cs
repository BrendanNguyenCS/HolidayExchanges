namespace HolidayExchanges.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreatorPropertyforGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "Creator", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Group", "Creator");
        }
    }
}

namespace HolidayExchanges.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingTimePartOfExchangeDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Group", "ExchangeDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Group", "ExchangeDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}

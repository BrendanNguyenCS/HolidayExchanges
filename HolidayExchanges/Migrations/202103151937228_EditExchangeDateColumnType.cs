namespace HolidayExchanges.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditExchangeDateColumnType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Group", "ExchangeDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Group", "ExchangeDate", c => c.DateTime());
        }
    }
}

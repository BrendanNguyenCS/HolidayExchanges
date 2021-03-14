namespace HolidayExchanges.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class EditWishProperties : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Wishes", "ItemName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Wishes", "Description", c => c.String(maxLength: 1000));
            AlterColumn("dbo.Wishes", "ItemLink", c => c.String());
            AlterColumn("dbo.Wishes", "PurchasingInstructions", c => c.String(maxLength: 1000));
            AlterColumn("dbo.Wishes", "HasBeenBought", c => c.Boolean(nullable: false, defaultValue: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.Wishes", "HasBeenBought", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Wishes", "PurchasingInstructions", c => c.String());
            AlterColumn("dbo.Wishes", "ItemLink", c => c.Int(nullable: false));
            AlterColumn("dbo.Wishes", "Description", c => c.String());
            AlterColumn("dbo.Wishes", "ItemName", c => c.String());
        }
    }
}
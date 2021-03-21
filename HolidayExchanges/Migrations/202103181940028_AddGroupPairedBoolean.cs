namespace HolidayExchanges.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddGroupPairedBoolean : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "HasBeenPaired", c => c.Boolean(nullable: false, defaultValue: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Group", "HasBeenPaired");
        }
    }
}
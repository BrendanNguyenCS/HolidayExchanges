namespace HolidayExchanges.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddWishes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Wishes",
                c => new
                {
                    WishID = c.Int(nullable: false, identity: true),
                    UserID = c.Int(nullable: false),
                    ItemName = c.String(),
                    Description = c.String(),
                    Quantity = c.Int(nullable: false),
                    ItemLink = c.Int(nullable: false),
                    PurchasingInstructions = c.String(),
                    HasBeenBought = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.WishID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Wishes", "UserID", "dbo.User");
            DropIndex("dbo.Wishes", new[] { "UserID" });
            DropTable("dbo.Wishes");
        }
    }
}
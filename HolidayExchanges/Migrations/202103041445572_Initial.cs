namespace HolidayExchanges.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Group",
                c => new
                    {
                        GroupID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                        ExchangeDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.GroupID);
            
            CreateTable(
                "dbo.UserGroup",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        GroupID = c.Int(nullable: false),
                        RecipientUserID = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserID, t.GroupID })
                .ForeignKey("dbo.Group", t => t.GroupID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID)
                .Index(t => t.GroupID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        HashedPassword = c.String(nullable: false, maxLength: 256),
                        Salt = c.String(maxLength: 128),
                        Email = c.String(maxLength: 128),
                        FirstName = c.String(maxLength: 20),
                        LastName = c.String(maxLength: 20),
                        Address1 = c.String(maxLength: 128),
                        Address2 = c.String(maxLength: 128),
                        City = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        Zip = c.String(maxLength: 10),
                        Country = c.String(maxLength: 50),
                        Birthday = c.DateTime(storeType: "date"),
                        PhoneNumber = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGroup", "UserID", "dbo.User");
            DropForeignKey("dbo.UserGroup", "GroupID", "dbo.Group");
            DropIndex("dbo.UserGroup", new[] { "GroupID" });
            DropIndex("dbo.UserGroup", new[] { "UserID" });
            DropTable("dbo.User");
            DropTable("dbo.UserGroup");
            DropTable("dbo.Group");
        }
    }
}

namespace LinkDib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactoring : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "DateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserNotifications", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.UserNotifications", "ApplicationUser_Id");
            AddForeignKey("dbo.UserNotifications", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserNotifications", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserNotifications", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.UserNotifications", "ApplicationUser_Id");
            DropColumn("dbo.Notifications", "DateTime");
        }
    }
}

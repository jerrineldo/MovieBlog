namespace MovieBlog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class March7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "ActorHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Actors", "PicExtension", c => c.String());
            AddColumn("dbo.Movies", "PlayerHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Movies", "PicExtension", c => c.String());
            AddColumn("dbo.Directors", "DirectorHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Directors", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Directors", "PicExtension");
            DropColumn("dbo.Directors", "DirectorHasPic");
            DropColumn("dbo.Movies", "PicExtension");
            DropColumn("dbo.Movies", "PlayerHasPic");
            DropColumn("dbo.Actors", "PicExtension");
            DropColumn("dbo.Actors", "ActorHasPic");
        }
    }
}

namespace MovieBlog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jerrin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "MovieHasPic", c => c.Boolean(nullable: false));
            DropColumn("dbo.Movies", "PlayerHasPic");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Movies", "PlayerHasPic", c => c.Boolean(nullable: false));
            DropColumn("dbo.Movies", "MovieHasPic");
        }
    }
}

namespace RecipeApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        DatabaseId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(),
                        IngredientsJson = c.String(nullable: false),
                        InstructionsJson = c.String(nullable: false),
                        ImageUrl = c.String(),
                        PrepTimeMinutes = c.Int(nullable: false),
                        CookTimeMinutes = c.Int(nullable: false),
                        Servings = c.Int(nullable: false),
                        CuisineType = c.String(),
                        Difficulty = c.String(),
                        IsFavourite = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                        Source = c.String(),
                    })
                .PrimaryKey(t => t.DatabaseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Recipes");
        }
    }
}

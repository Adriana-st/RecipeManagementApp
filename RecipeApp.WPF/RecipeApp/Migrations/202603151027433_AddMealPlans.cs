namespace RecipeApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMealPlans : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MealPlans",
                c => new
                    {
                        MealPlanId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        MealType = c.String(nullable: false, maxLength: 50),
                        RecipeId = c.Int(nullable: false),
                        RecipeName = c.String(nullable: false, maxLength: 200),
                        Notes = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MealPlanId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MealPlans");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatePlanner.Infrastructure.Database.Migrations
{
    public partial class RetireDateIdeas : Migration
    {
        // Retire the EF model while preserving legacy idea tables and their data in both directions.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatePlanner.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCoupleWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_turn_games_couple_id",
                schema: "dateplanner",
                table: "turn_games");

            migrationBuilder.AlterColumn<string>(
                name: "game_type",
                schema: "dateplanner",
                table: "turn_games",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "purpose",
                schema: "dateplanner",
                table: "turn_games",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Casual");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at_utc",
                schema: "dateplanner",
                table: "date_plans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "is_collaborative",
                schema: "dateplanner",
                table: "date_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_turn_games_active_planning_decision",
                schema: "dateplanner",
                table: "turn_games",
                column: "couple_id",
                unique: true,
                filter: "purpose = 'PlanningDecision' AND status <> 'Finished'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_turn_games_active_planning_decision",
                schema: "dateplanner",
                table: "turn_games");

            migrationBuilder.DropColumn(
                name: "purpose",
                schema: "dateplanner",
                table: "turn_games");

            migrationBuilder.DropColumn(
                name: "created_at_utc",
                schema: "dateplanner",
                table: "date_plans");

            migrationBuilder.DropColumn(
                name: "is_collaborative",
                schema: "dateplanner",
                table: "date_plans");

            migrationBuilder.AlterColumn<string>(
                name: "game_type",
                schema: "dateplanner",
                table: "turn_games",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_turn_games_couple_id",
                schema: "dateplanner",
                table: "turn_games",
                column: "couple_id");
        }
    }
}

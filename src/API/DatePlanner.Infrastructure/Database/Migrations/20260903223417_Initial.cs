using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatePlanner.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dateplanner");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    partner_code = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "couples",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    partner_one_id = table.Column<Guid>(type: "uuid", nullable: false),
                    partner_two_id = table.Column<Guid>(type: "uuid", nullable: false),
                    begin_relationship_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_couples", x => x.id);
                    table.CheckConstraint("ck_couples_different_partners", "partner_one_id <> partner_two_id");
                    table.ForeignKey(
                        name: "fk_couples_users_partner_one_id",
                        column: x => x.partner_one_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_couples_users_partner_two_id",
                        column: x => x.partner_two_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dating_preferences",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    avoided_activities = table.Column<List<string>>(type: "text[]", nullable: false),
                    favorite_activities = table.Column<List<string>>(type: "text[]", nullable: false),
                    food_preferences = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dating_preferences", x => x.id);
                    table.ForeignKey(
                        name: "fk_dating_preferences_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "date_plans",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    couple_id = table.Column<Guid>(type: "uuid", nullable: false),
                    partner_who_planned_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_for = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_date_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_date_plans_couples_couple_id",
                        column: x => x.couple_id,
                        principalSchema: "dateplanner",
                        principalTable: "couples",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_date_plans_users_partner_who_planned_id",
                        column: x => x.partner_who_planned_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "turn_games",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    couple_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_one_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_two_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    winner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    started_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    finished_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_turn_games", x => x.id);
                    table.CheckConstraint("ck_turn_games_different_players", "player_one_id <> player_two_id");
                    table.CheckConstraint("ck_turn_games_valid_winner", "winner_id IS NULL OR winner_id = player_one_id OR winner_id = player_two_id");
                    table.ForeignKey(
                        name: "fk_turn_games_couples_couple_id",
                        column: x => x.couple_id,
                        principalSchema: "dateplanner",
                        principalTable: "couples",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_turn_games_users_player_one_id",
                        column: x => x.player_one_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_turn_games_users_player_two_id",
                        column: x => x.player_two_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_turn_games_users_winner_id",
                        column: x => x.winner_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "date_ideas",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    couple_id = table.Column<Guid>(type: "uuid", nullable: false),
                    result_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    preferred_budget = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    estimated_budget = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    city = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    starts_at = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ends_at = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_date_ideas", x => x.id);
                    table.CheckConstraint("ck_date_ideas_estimated_budget_non_negative", "estimated_budget IS NULL OR estimated_budget >= 0");
                    table.CheckConstraint("ck_date_ideas_preferred_budget_non_negative", "preferred_budget >= 0");
                    table.ForeignKey(
                        name: "fk_date_ideas_couples_couple_id",
                        column: x => x.couple_id,
                        principalSchema: "dateplanner",
                        principalTable: "couples",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_date_ideas_date_plans_result_plan",
                        column: x => x.result_plan_id,
                        principalSchema: "dateplanner",
                        principalTable: "date_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "itinerary_items",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    starts_at = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: true),
                    date_plan_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_itinerary_items", x => x.id);
                    table.CheckConstraint("ck_itinerary_items_order_positive", "\"order\" > 0");
                    table.ForeignKey(
                        name: "fk_itinerary_items_date_plans_date_plan_id",
                        column: x => x.date_plan_id,
                        principalSchema: "dateplanner",
                        principalTable: "date_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "turn_game_rounds",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    turn_game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    winner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    started_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_turn_game_rounds", x => x.id);
                    table.CheckConstraint("ck_turn_game_rounds_number_positive", "number > 0");
                    table.ForeignKey(
                        name: "fk_turn_game_rounds_turn_games_turn_game_id",
                        column: x => x.turn_game_id,
                        principalSchema: "dateplanner",
                        principalTable: "turn_games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_turn_game_rounds_users_winner_id",
                        column: x => x.winner_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "date_idea_itinerary_items",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    starts_at = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: true),
                    date_idea_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_date_idea_itinerary_items", x => x.id);
                    table.CheckConstraint("ck_date_idea_itinerary_items_order_positive", "\"order\" > 0");
                    table.ForeignKey(
                        name: "fk_date_idea_itinerary_items_date_ideas_date_idea_id",
                        column: x => x.date_idea_id,
                        principalSchema: "dateplanner",
                        principalTable: "date_ideas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "turn_game_moves",
                schema: "dateplanner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    turn_game_round_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    played_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_turn_game_moves", x => x.id);
                    table.CheckConstraint("ck_turn_game_moves_sequence_positive", "sequence > 0");
                    table.ForeignKey(
                        name: "fk_turn_game_moves_turn_game_rounds_turn_game_round_id",
                        column: x => x.turn_game_round_id,
                        principalSchema: "dateplanner",
                        principalTable: "turn_game_rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_turn_game_moves_users_player_id",
                        column: x => x.player_id,
                        principalSchema: "dateplanner",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_couples_partner_one_id",
                schema: "dateplanner",
                table: "couples",
                column: "partner_one_id");

            migrationBuilder.CreateIndex(
                name: "ix_couples_partner_two_id",
                schema: "dateplanner",
                table: "couples",
                column: "partner_two_id");

            migrationBuilder.CreateIndex(
                name: "ix_date_idea_itinerary_items_date_idea_id_order",
                schema: "dateplanner",
                table: "date_idea_itinerary_items",
                columns: new[] { "date_idea_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_date_ideas_couple_id",
                schema: "dateplanner",
                table: "date_ideas",
                column: "couple_id");

            migrationBuilder.CreateIndex(
                name: "ix_date_ideas_result_plan",
                schema: "dateplanner",
                table: "date_ideas",
                column: "result_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_date_plans_couple_id",
                schema: "dateplanner",
                table: "date_plans",
                column: "couple_id");

            migrationBuilder.CreateIndex(
                name: "ix_date_plans_partner_who_planned_id",
                schema: "dateplanner",
                table: "date_plans",
                column: "partner_who_planned_id");

            migrationBuilder.CreateIndex(
                name: "ix_dating_preferences_user_id",
                schema: "dateplanner",
                table: "dating_preferences",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_itinerary_items_date_plan_id_order",
                schema: "dateplanner",
                table: "itinerary_items",
                columns: new[] { "date_plan_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_turn_game_moves_player_id",
                schema: "dateplanner",
                table: "turn_game_moves",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_turn_game_moves_turn_game_round_id_player_id",
                schema: "dateplanner",
                table: "turn_game_moves",
                columns: new[] { "turn_game_round_id", "player_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_turn_game_moves_turn_game_round_id_sequence",
                schema: "dateplanner",
                table: "turn_game_moves",
                columns: new[] { "turn_game_round_id", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_turn_game_rounds_turn_game_id_number",
                schema: "dateplanner",
                table: "turn_game_rounds",
                columns: new[] { "turn_game_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_turn_game_rounds_winner_id",
                schema: "dateplanner",
                table: "turn_game_rounds",
                column: "winner_id");

            migrationBuilder.CreateIndex(
                name: "ix_turn_games_couple_id",
                schema: "dateplanner",
                table: "turn_games",
                column: "couple_id");

            migrationBuilder.CreateIndex(
                name: "ix_turn_games_player_one_id",
                schema: "dateplanner",
                table: "turn_games",
                column: "player_one_id");

            migrationBuilder.CreateIndex(
                name: "ix_turn_games_player_two_id",
                schema: "dateplanner",
                table: "turn_games",
                column: "player_two_id");

            migrationBuilder.CreateIndex(
                name: "ix_turn_games_winner_id",
                schema: "dateplanner",
                table: "turn_games",
                column: "winner_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "dateplanner",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_partner_code",
                schema: "dateplanner",
                table: "users",
                column: "partner_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "date_idea_itinerary_items",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "dating_preferences",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "itinerary_items",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "turn_game_moves",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "date_ideas",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "turn_game_rounds",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "date_plans",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "turn_games",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "couples",
                schema: "dateplanner");

            migrationBuilder.DropTable(
                name: "users",
                schema: "dateplanner");
        }
    }
}

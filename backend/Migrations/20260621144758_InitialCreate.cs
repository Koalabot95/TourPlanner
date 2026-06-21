using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "tours",
                columns: table => new
                {
                    tour_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    end_location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    transport_type = table.Column<string>(type: "text", nullable: false),
                    distance = table.Column<double>(type: "double precision", nullable: true),
                    estimated_time = table.Column<double>(type: "double precision", nullable: true),
                    route_information = table.Column<string>(type: "text", nullable: true),
                    map_snapshot_path = table.Column<string>(type: "text", nullable: true),
                    popularity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    child_friendliness = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tours", x => x.tour_id);
                    table.ForeignKey(
                        name: "FK_tours_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tour_logs",
                columns: table => new
                {
                    log_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tour_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    difficulty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    total_distance = table.Column<double>(type: "double precision", nullable: false),
                    total_time = table.Column<double>(type: "double precision", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_logs", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_tour_logs_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "tour_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    image_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    file_path = table.Column<string>(type: "text", nullable: false),
                    caption = table.Column<string>(type: "text", nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    log_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tour_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_images_tour_logs_log_id",
                        column: x => x.log_id,
                        principalTable: "tour_logs",
                        principalColumn: "log_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_images_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "tour_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_images_log_id",
                table: "images",
                column: "log_id");

            migrationBuilder.CreateIndex(
                name: "IX_images_tour_id",
                table: "images",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_logs_tour_id",
                table: "tour_logs",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_tours_user_id",
                table: "tours",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "tour_logs");

            migrationBuilder.DropTable(
                name: "tours");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

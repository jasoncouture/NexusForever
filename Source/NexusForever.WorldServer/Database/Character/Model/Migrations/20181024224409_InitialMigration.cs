using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Database.Character.Model.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false, defaultValueSql: "'0'"),
                    accountId = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    name = table.Column<string>(type: "varchar(50)", nullable: false, defaultValueSql: "''"),
                    sex = table.Column<byte>(nullable: false, defaultValueSql: "'0'"),
                    race = table.Column<byte>(nullable: false, defaultValueSql: "'0'"),
                    @class = table.Column<byte>(name: "class", nullable: false, defaultValueSql: "'0'"),
                    level = table.Column<byte>(nullable: false, defaultValueSql: "'0'"),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: migrationBuilder.IsMySql() ? "CURRENT_TIMESTAMP" : "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character_appearance",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false, defaultValueSql: "'0'"),
                    slot = table.Column<byte>(nullable: false, defaultValueSql: "'0'"),
                    displayId = table.Column<int>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_appearance", x => new { x.id, x.slot });
                    table.ForeignKey(
                        name: "FK__character_appearance_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_customisation",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false, defaultValueSql: "'0'"),
                    label = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    value = table.Column<long>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_customisation", x => new { x.id, x.label });
                    table.ForeignKey(
                        name: "FK__character_customisation_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false, defaultValueSql: "'0'"),
                    ownerId = table.Column<decimal>(nullable: false, defaultValueSql: "'0'"),
                    itemId = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    location = table.Column<int>(nullable: false, defaultValueSql: "'0'"),
                    bagIndex = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    stackCount = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    charges = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    durability = table.Column<float>(nullable: false, defaultValueSql: "'0'"),
                    expirationTimeLeft = table.Column<long>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item", x => x.id);
                    table.ForeignKey(
                        name: "FK__item_ownerId__character_id",
                        column: x => x.ownerId,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "accountId",
                table: "character",
                column: "accountId");

            migrationBuilder.CreateIndex(
                name: "FK__item_ownerId__character_id",
                table: "item",
                column: "ownerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_appearance");

            migrationBuilder.DropTable(
                name: "character_customisation");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "character");
        }
    }
}

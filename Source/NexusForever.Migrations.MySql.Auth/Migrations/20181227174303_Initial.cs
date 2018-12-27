using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace NexusForever.Migrations.MySql.Auth.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(128)", nullable: false, defaultValueSql: "''"),
                    s = table.Column<string>(type: "varchar(32)", nullable: false, defaultValueSql: "''"),
                    v = table.Column<string>(type: "varchar(512)", nullable: false, defaultValueSql: "''"),
                    gameToken = table.Column<string>(type: "varchar(32)", nullable: false, defaultValueSql: "''"),
                    sessionKey = table.Column<string>(type: "varchar(32)", nullable: false, defaultValueSql: "''"),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "server",
                columns: table => new
                {
                    id = table.Column<byte>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(64)", nullable: false, defaultValueSql: "'NexusForever'"),
                    host = table.Column<string>(type: "varchar(64)", nullable: false, defaultValueSql: "'127.0.0.1'"),
                    port = table.Column<ushort>(nullable: false, defaultValueSql: "'24000'"),
                    type = table.Column<byte>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "email",
                table: "account",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "gameToken",
                table: "account",
                column: "gameToken");

            migrationBuilder.CreateIndex(
                name: "sessionKey",
                table: "account",
                column: "sessionKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "server");
        }
    }
}

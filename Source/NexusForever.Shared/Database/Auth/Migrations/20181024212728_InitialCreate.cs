using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Shared.Database.Auth.Migrations
{
    public partial class InitialCreate : Migration
    {
        private string GetDateTimeNowValueForProvider(string provider)
        {
            switch (provider)
            {
                    case "Pomelo.EntityFrameworkCore.MySql":
                        return "CURRENT_TIMESTAMP";
                    case "Microsoft.EntityFrameworkCore.SqlServer":
                        return "SYSDATETIME()";
                    default:
                        return "NOW()";
            }
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false),
                    email = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, defaultValueSql: "''"),
                    s = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, defaultValueSql: "''"),
                    v = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false, defaultValueSql: "''"),
                    gameToken = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, defaultValueSql: "''"),
                    sessionKey = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, defaultValueSql: "''"),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: GetDateTimeNowValueForProvider(migrationBuilder.ActiveProvider))
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
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValueSql: "'NexusForever'"),
                    host = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValueSql: "'127.0.0.1'"),
                    port = table.Column<int>(nullable: false, defaultValueSql: "'24000'"),
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

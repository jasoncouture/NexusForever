using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.WorldServer.Database.World.Model.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entity",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    type = table.Column<byte>(nullable: false, defaultValueSql: "'0'"),
                    creature = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    world = table.Column<int>(nullable: false, defaultValueSql: "'0'"),
                    x = table.Column<float>(nullable: false, defaultValueSql: "'0'"),
                    y = table.Column<float>(nullable: false, defaultValueSql: "'0'"),
                    z = table.Column<float>(nullable: false, defaultValueSql: "'0'"),
                    rx = table.Column<float>(nullable: false, defaultValueSql: "'0'"),
                    ry = table.Column<float>(nullable: false, defaultValueSql: "'0'"),
                    rz = table.Column<float>(nullable: false, defaultValueSql: "'0'"),
                    displayInfo = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    outfitInfo = table.Column<int>(nullable: false, defaultValueSql: "'0'"),
                    faction1 = table.Column<int>(nullable: false, defaultValueSql: "'0'"),
                    faction2 = table.Column<int>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    buyPriceMultiplier = table.Column<float>(nullable: false, defaultValueSql: "'1'"),
                    sellPriceMultiplier = table.Column<float>(nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_vendor", x => x.id);
                    table.ForeignKey(
                        name: "FK__entity_vendor_id__entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor_category",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    index = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    localisedTextId = table.Column<long>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_vendor_category", x => new { x.id, x.index });
                    table.ForeignKey(
                        name: "FK__entity_vendor_category_id__entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor_item",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    index = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    categoryIndex = table.Column<long>(nullable: false, defaultValueSql: "'0'"),
                    itemId = table.Column<long>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_vendor_item", x => new { x.id, x.index });
                    table.ForeignKey(
                        name: "FK__entity_vendor_item_id__entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity_vendor");

            migrationBuilder.DropTable(
                name: "entity_vendor_category");

            migrationBuilder.DropTable(
                name: "entity_vendor_item");

            migrationBuilder.DropTable(
                name: "entity");
        }
    }
}

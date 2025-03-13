using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Db.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KRAJ",
                columns: table => new
                {
                    KRAJ = table.Column<int>(type: "int", maxLength: 5, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NRAJ = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KRAJ", x => x.KRAJ);
                });

            migrationBuilder.CreateTable(
                name: "OBL",
                columns: table => new
                {
                    OBL = table.Column<short>(type: "smallint", maxLength: 4, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOBL = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OBL", x => x.OBL);
                });

            migrationBuilder.CreateTable(
                name: "CITY",
                columns: table => new
                {
                    CITY_KOD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CITY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KRAJ = table.Column<int>(type: "int", maxLength: 5, nullable: true),
                    OBL = table.Column<short>(type: "smallint", maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CITY", x => x.CITY_KOD);
                    table.ForeignKey(
                        name: "FK_CITY_KRAJ_KRAJ",
                        column: x => x.KRAJ,
                        principalTable: "KRAJ",
                        principalColumn: "KRAJ");
                    table.ForeignKey(
                        name: "FK_CITY_OBL_OBL",
                        column: x => x.OBL,
                        principalTable: "OBL",
                        principalColumn: "OBL");
                });

            migrationBuilder.CreateTable(
                name: "AUP",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", maxLength: 10, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    INDEX_A = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CITY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NCITY = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OBL = table.Column<short>(type: "smallint", maxLength: 4, nullable: true),
                    NOBL = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RAJ = table.Column<int>(type: "int", maxLength: 5, nullable: true),
                    NRAJ = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUP", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AUP_CITY_CITY",
                        column: x => x.CITY,
                        principalTable: "CITY",
                        principalColumn: "CITY_KOD");
                    table.ForeignKey(
                        name: "FK_AUP_KRAJ_RAJ",
                        column: x => x.RAJ,
                        principalTable: "KRAJ",
                        principalColumn: "KRAJ");
                    table.ForeignKey(
                        name: "FK_AUP_OBL_OBL",
                        column: x => x.OBL,
                        principalTable: "OBL",
                        principalColumn: "OBL");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUP_CITY",
                table: "AUP",
                column: "CITY");

            migrationBuilder.CreateIndex(
                name: "IX_AUP_OBL",
                table: "AUP",
                column: "OBL");

            migrationBuilder.CreateIndex(
                name: "IX_AUP_RAJ",
                table: "AUP",
                column: "RAJ");

            migrationBuilder.CreateIndex(
                name: "IX_CITY_KRAJ",
                table: "CITY",
                column: "KRAJ");

            migrationBuilder.CreateIndex(
                name: "IX_CITY_OBL",
                table: "CITY",
                column: "OBL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUP");

            migrationBuilder.DropTable(
                name: "CITY");

            migrationBuilder.DropTable(
                name: "KRAJ");

            migrationBuilder.DropTable(
                name: "OBL");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations.SqlServerMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    PersonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserAdded = table.Column<int>(nullable: true),
                    UserModified = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    MiddleInitial = table.Column<string>(nullable: true),
                    FormattedName = table.Column<string>(nullable: true),
                    BaptismDate = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNo = table.Column<string>(nullable: true),
                    MobileNo = table.Column<string>(nullable: true),
                    CongregationId = table.Column<int>(nullable: true),
                    AddressId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.PersonId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "People");
        }
    }
}

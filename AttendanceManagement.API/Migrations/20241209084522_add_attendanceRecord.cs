using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class add_attendanceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Barcodes_BarcodeId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_BarcodeId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "BarcodeId",
                table: "Attendances");

            migrationBuilder.CreateTable(
                name: "AttendanceRecord",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkerId = table.Column<int>(type: "int", nullable: false),
                    BarcodeId = table.Column<int>(type: "int", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecord", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_Barcodes_BarcodeId",
                        column: x => x.BarcodeId,
                        principalTable: "Barcodes",
                        principalColumn: "BarcodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_Users_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_BarcodeId",
                table: "AttendanceRecord",
                column: "BarcodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_WorkerId",
                table: "AttendanceRecord",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRecord");

            migrationBuilder.AddColumn<int>(
                name: "BarcodeId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_BarcodeId",
                table: "Attendances",
                column: "BarcodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Barcodes_BarcodeId",
                table: "Attendances",
                column: "BarcodeId",
                principalTable: "Barcodes",
                principalColumn: "BarcodeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

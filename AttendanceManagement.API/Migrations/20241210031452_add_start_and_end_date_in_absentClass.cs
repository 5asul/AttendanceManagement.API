using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class add_start_and_end_date_in_absentClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Absences",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Absences",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Absences");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Absences",
                newName: "Date");
        }
    }
}

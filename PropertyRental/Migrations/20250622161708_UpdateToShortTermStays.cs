using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyRental.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToShortTermStays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateRented",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "PricePerMonth",
                table: "Properties",
                newName: "PricePerNight");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_PricePerMonth",
                table: "Properties",
                newName: "IX_Properties_PricePerNight");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckInTime",
                table: "Properties",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckOutTime",
                table: "Properties",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "HasAirConditioning",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasKitchen",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasWasher",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasWifi",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxGuests",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxStayNights",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinStayNights",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalGuests",
                table: "Bookings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Bookings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_MaxGuests",
                table: "Properties",
                column: "MaxGuests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_MaxGuests",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HasAirConditioning",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HasKitchen",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HasWasher",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HasWifi",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MaxGuests",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MaxStayNights",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MinStayNights",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "TotalGuests",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "PricePerNight",
                table: "Properties",
                newName: "PricePerMonth");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_PricePerNight",
                table: "Properties",
                newName: "IX_Properties_PricePerMonth");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRented",
                table: "Properties",
                type: "TEXT",
                nullable: true);
        }
    }
}

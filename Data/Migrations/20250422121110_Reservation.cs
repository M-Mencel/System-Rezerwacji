using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace System_Rezerwacji.Data.Migrations
{
    /// <inheritdoc />
    public partial class Reservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Customer_CustomerID",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Service_ServiceID",
                table: "Reservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation");

            migrationBuilder.RenameTable(
                name: "Reservation",
                newName: "Reservations");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_ServiceID",
                table: "Reservations",
                newName: "IX_Reservations_ServiceID");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_CustomerID",
                table: "Reservations",
                newName: "IX_Reservations_CustomerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "ReservationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Customer_CustomerID",
                table: "Reservations",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Service_ServiceID",
                table: "Reservations",
                column: "ServiceID",
                principalTable: "Service",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Customer_CustomerID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Service_ServiceID",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "Reservation");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_ServiceID",
                table: "Reservation",
                newName: "IX_Reservation_ServiceID");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_CustomerID",
                table: "Reservation",
                newName: "IX_Reservation_CustomerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation",
                column: "ReservationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Customer_CustomerID",
                table: "Reservation",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Service_ServiceID",
                table: "Reservation",
                column: "ServiceID",
                principalTable: "Service",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

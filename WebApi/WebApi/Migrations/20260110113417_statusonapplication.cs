using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class statusonapplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateApplications_RoundType_RoundTypeId",
                table: "CandidateApplications");

            migrationBuilder.DropIndex(
                name: "IX_CandidateApplications_RoundTypeId",
                table: "CandidateApplications");

            migrationBuilder.DropColumn(
                name: "IsOnHold",
                table: "CandidateApplications");

            migrationBuilder.DropColumn(
                name: "RoundTypeId",
                table: "CandidateApplications");

            migrationBuilder.RenameColumn(
                name: "OnHoldReason",
                table: "CandidateApplications",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "CandidateApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "CandidateApplications");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "CandidateApplications",
                newName: "OnHoldReason");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnHold",
                table: "CandidateApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RoundTypeId",
                table: "CandidateApplications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_RoundTypeId",
                table: "CandidateApplications",
                column: "RoundTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateApplications_RoundType_RoundTypeId",
                table: "CandidateApplications",
                column: "RoundTypeId",
                principalTable: "RoundType",
                principalColumn: "RoundTypeId");
        }
    }
}

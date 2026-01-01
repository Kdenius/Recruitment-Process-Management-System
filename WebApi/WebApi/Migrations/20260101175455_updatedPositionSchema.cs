using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updatedPositionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateApplications_Position_PositionId",
                table: "CandidateApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_Position_Users_RecruiterId",
                table: "Position");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionSkills_Position_PositionId",
                table: "PositionSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Position",
                table: "Position");

            migrationBuilder.RenameTable(
                name: "Position",
                newName: "Positions");

            migrationBuilder.RenameIndex(
                name: "IX_Position_RecruiterId",
                table: "Positions",
                newName: "IX_Positions_RecruiterId");

            migrationBuilder.AlterColumn<string>(
                name: "CloserReason",
                table: "Positions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Positions",
                table: "Positions",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateApplications_Positions_PositionId",
                table: "CandidateApplications",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Users_RecruiterId",
                table: "Positions",
                column: "RecruiterId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionSkills_Positions_PositionId",
                table: "PositionSkills",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateApplications_Positions_PositionId",
                table: "CandidateApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Users_RecruiterId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionSkills_Positions_PositionId",
                table: "PositionSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Positions",
                table: "Positions");

            migrationBuilder.RenameTable(
                name: "Positions",
                newName: "Position");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_RecruiterId",
                table: "Position",
                newName: "IX_Position_RecruiterId");

            migrationBuilder.AlterColumn<string>(
                name: "CloserReason",
                table: "Position",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Position",
                table: "Position",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateApplications_Position_PositionId",
                table: "CandidateApplications",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Position_Users_RecruiterId",
                table: "Position",
                column: "RecruiterId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionSkills_Position_PositionId",
                table: "PositionSkills",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

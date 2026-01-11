using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class fixduplicateForeingkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewRounds_CandidateApplications_CandidateApplicationApplicationId",
                table: "InterviewRounds");

            migrationBuilder.DropIndex(
                name: "IX_InterviewRounds_CandidateApplicationApplicationId",
                table: "InterviewRounds");

            migrationBuilder.DropColumn(
                name: "CandidateApplicationApplicationId",
                table: "InterviewRounds");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRounds_ApplicationId",
                table: "InterviewRounds",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewRounds_CandidateApplications_ApplicationId",
                table: "InterviewRounds",
                column: "ApplicationId",
                principalTable: "CandidateApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewRounds_CandidateApplications_ApplicationId",
                table: "InterviewRounds");

            migrationBuilder.DropIndex(
                name: "IX_InterviewRounds_ApplicationId",
                table: "InterviewRounds");

            migrationBuilder.AddColumn<int>(
                name: "CandidateApplicationApplicationId",
                table: "InterviewRounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRounds_CandidateApplicationApplicationId",
                table: "InterviewRounds",
                column: "CandidateApplicationApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewRounds_CandidateApplications_CandidateApplicationApplicationId",
                table: "InterviewRounds",
                column: "CandidateApplicationApplicationId",
                principalTable: "CandidateApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

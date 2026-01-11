using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class fixduplicateForeingkey2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateDocuments_CandidateApplications_CandidateApplicationApplicationId",
                table: "CandidateDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateDocuments_DocumentTypes_DocumentTypeDocTypeId",
                table: "CandidateDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_InterviewRounds_InterviewRoundRoundId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferLetters_CandidateApplications_CandidateApplicationApplicationId",
                table: "OfferLetters");

            migrationBuilder.DropIndex(
                name: "IX_OfferLetters_CandidateApplicationApplicationId",
                table: "OfferLetters");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_InterviewRoundRoundId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_CandidateDocuments_CandidateApplicationApplicationId",
                table: "CandidateDocuments");

            migrationBuilder.DropIndex(
                name: "IX_CandidateDocuments_DocumentTypeDocTypeId",
                table: "CandidateDocuments");

            migrationBuilder.DropColumn(
                name: "CandidateApplicationApplicationId",
                table: "OfferLetters");

            migrationBuilder.DropColumn(
                name: "InterviewRoundRoundId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "CandidateApplicationApplicationId",
                table: "CandidateDocuments");

            migrationBuilder.DropColumn(
                name: "DocumentTypeDocTypeId",
                table: "CandidateDocuments");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLetters_ApplicationId",
                table: "OfferLetters",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_RoundId",
                table: "Interviews",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateDocuments_ApplicationId",
                table: "CandidateDocuments",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateDocuments_DocTypeId",
                table: "CandidateDocuments",
                column: "DocTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateDocuments_CandidateApplications_ApplicationId",
                table: "CandidateDocuments",
                column: "ApplicationId",
                principalTable: "CandidateApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateDocuments_DocumentTypes_DocTypeId",
                table: "CandidateDocuments",
                column: "DocTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "DocTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_InterviewRounds_RoundId",
                table: "Interviews",
                column: "RoundId",
                principalTable: "InterviewRounds",
                principalColumn: "RoundId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferLetters_CandidateApplications_ApplicationId",
                table: "OfferLetters",
                column: "ApplicationId",
                principalTable: "CandidateApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateDocuments_CandidateApplications_ApplicationId",
                table: "CandidateDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateDocuments_DocumentTypes_DocTypeId",
                table: "CandidateDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_InterviewRounds_RoundId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferLetters_CandidateApplications_ApplicationId",
                table: "OfferLetters");

            migrationBuilder.DropIndex(
                name: "IX_OfferLetters_ApplicationId",
                table: "OfferLetters");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_RoundId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_CandidateDocuments_ApplicationId",
                table: "CandidateDocuments");

            migrationBuilder.DropIndex(
                name: "IX_CandidateDocuments_DocTypeId",
                table: "CandidateDocuments");

            migrationBuilder.AddColumn<int>(
                name: "CandidateApplicationApplicationId",
                table: "OfferLetters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InterviewRoundRoundId",
                table: "Interviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CandidateApplicationApplicationId",
                table: "CandidateDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeDocTypeId",
                table: "CandidateDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OfferLetters_CandidateApplicationApplicationId",
                table: "OfferLetters",
                column: "CandidateApplicationApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewRoundRoundId",
                table: "Interviews",
                column: "InterviewRoundRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateDocuments_CandidateApplicationApplicationId",
                table: "CandidateDocuments",
                column: "CandidateApplicationApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateDocuments_DocumentTypeDocTypeId",
                table: "CandidateDocuments",
                column: "DocumentTypeDocTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateDocuments_CandidateApplications_CandidateApplicationApplicationId",
                table: "CandidateDocuments",
                column: "CandidateApplicationApplicationId",
                principalTable: "CandidateApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateDocuments_DocumentTypes_DocumentTypeDocTypeId",
                table: "CandidateDocuments",
                column: "DocumentTypeDocTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "DocTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_InterviewRounds_InterviewRoundRoundId",
                table: "Interviews",
                column: "InterviewRoundRoundId",
                principalTable: "InterviewRounds",
                principalColumn: "RoundId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferLetters_CandidateApplications_CandidateApplicationApplicationId",
                table: "OfferLetters",
                column: "CandidateApplicationApplicationId",
                principalTable: "CandidateApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

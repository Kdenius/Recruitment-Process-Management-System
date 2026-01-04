using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitiDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    CandidateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResumeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.CandidateId);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    DocTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.DocTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "RoundType",
                columns: table => new
                {
                    RoundTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundType", x => x.RoundTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifyToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateSkills",
                columns: table => new
                {
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    CandidateSkillId = table.Column<int>(type: "int", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateSkills", x => new { x.CandidateId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_CandidateSkills_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "CandidateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseSalary = table.Column<int>(type: "int", nullable: false),
                    MaxSalary = table.Column<int>(type: "int", nullable: false),
                    CloserReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rounds = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecruiterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                    table.ForeignKey(
                        name: "FK_Positions_Users_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateApplications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    IsOnHold = table.Column<bool>(type: "bit", nullable: false),
                    OnHoldReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoundTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateApplications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_CandidateApplications_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "CandidateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateApplications_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateApplications_RoundType_RoundTypeId",
                        column: x => x.RoundTypeId,
                        principalTable: "RoundType",
                        principalColumn: "RoundTypeId");
                });

            migrationBuilder.CreateTable(
                name: "PositionSkills",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    PositionSkillId = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionSkills", x => new { x.PositionId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_PositionSkills_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    CandidateApplicationApplicationId = table.Column<int>(type: "int", nullable: false),
                    DocTypeId = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeDocTypeId = table.Column<int>(type: "int", nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateDocuments", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_CandidateDocuments_CandidateApplications_CandidateApplicationApplicationId",
                        column: x => x.CandidateApplicationApplicationId,
                        principalTable: "CandidateApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateDocuments_DocumentTypes_DocumentTypeDocTypeId",
                        column: x => x.DocumentTypeDocTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "DocTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewRounds",
                columns: table => new
                {
                    RoundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    CandidateApplicationApplicationId = table.Column<int>(type: "int", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    RoundTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewRounds", x => x.RoundId);
                    table.ForeignKey(
                        name: "FK_InterviewRounds_CandidateApplications_CandidateApplicationApplicationId",
                        column: x => x.CandidateApplicationApplicationId,
                        principalTable: "CandidateApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewRounds_RoundType_RoundTypeId",
                        column: x => x.RoundTypeId,
                        principalTable: "RoundType",
                        principalColumn: "RoundTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferLetters",
                columns: table => new
                {
                    OfferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    CandidateApplicationApplicationId = table.Column<int>(type: "int", nullable: false),
                    SalaryOffered = table.Column<int>(type: "int", nullable: false),
                    OfferUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentByUserId = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferLetters", x => x.OfferId);
                    table.ForeignKey(
                        name: "FK_OfferLetters_CandidateApplications_CandidateApplicationApplicationId",
                        column: x => x.CandidateApplicationApplicationId,
                        principalTable: "CandidateApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferLetters_Users_SentByUserId",
                        column: x => x.SentByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    InterviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoundId = table.Column<int>(type: "int", nullable: false),
                    InterviewRoundRoundId = table.Column<int>(type: "int", nullable: false),
                    InterviewerId = table.Column<int>(type: "int", nullable: false),
                    FeedbackText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeedbackScore = table.Column<int>(type: "int", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.InterviewId);
                    table.ForeignKey(
                        name: "FK_Interviews_InterviewRounds_InterviewRoundRoundId",
                        column: x => x.InterviewRoundRoundId,
                        principalTable: "InterviewRounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interviews_Users_InterviewerId",
                        column: x => x.InterviewerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "InterviewRatings",
                columns: table => new
                {
                    InterviewId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    RatingId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewRatings", x => new { x.InterviewId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_InterviewRatings_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "InterviewId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewRatings_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_CandidateId",
                table: "CandidateApplications",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_PositionId",
                table: "CandidateApplications",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_RoundTypeId",
                table: "CandidateApplications",
                column: "RoundTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateDocuments_CandidateApplicationApplicationId",
                table: "CandidateDocuments",
                column: "CandidateApplicationApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateDocuments_DocumentTypeDocTypeId",
                table: "CandidateDocuments",
                column: "DocumentTypeDocTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSkills_SkillId",
                table: "CandidateSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRatings_SkillId",
                table: "InterviewRatings",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRounds_CandidateApplicationApplicationId",
                table: "InterviewRounds",
                column: "CandidateApplicationApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRounds_RoundTypeId",
                table: "InterviewRounds",
                column: "RoundTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewerId",
                table: "Interviews",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewRoundRoundId",
                table: "Interviews",
                column: "InterviewRoundRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLetters_CandidateApplicationApplicationId",
                table: "OfferLetters",
                column: "CandidateApplicationApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLetters_SentByUserId",
                table: "OfferLetters",
                column: "SentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_RecruiterId",
                table: "Positions",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionSkills_SkillId",
                table: "PositionSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateDocuments");

            migrationBuilder.DropTable(
                name: "CandidateSkills");

            migrationBuilder.DropTable(
                name: "InterviewRatings");

            migrationBuilder.DropTable(
                name: "OfferLetters");

            migrationBuilder.DropTable(
                name: "PositionSkills");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "InterviewRounds");

            migrationBuilder.DropTable(
                name: "CandidateApplications");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "RoundType");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}

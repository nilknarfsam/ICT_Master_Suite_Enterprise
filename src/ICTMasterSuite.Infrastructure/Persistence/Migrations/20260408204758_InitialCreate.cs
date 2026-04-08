using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ICTMasterSuite.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnowledgeBaseArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TestPhase = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Symptom = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Solution = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBaseArticles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Module = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    LogType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ErrorDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TechnicianName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AnalysisText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalAnalyses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "Module", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000001-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7955), 1, "Authentication.View", null },
                    { new Guid("00000001-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7961), 1, "Authentication.Manage", null },
                    { new Guid("00000002-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7963), 2, "UsersAndProfiles.View", null },
                    { new Guid("00000002-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7965), 2, "UsersAndProfiles.Manage", null },
                    { new Guid("00000003-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7967), 3, "LogFinder.View", null },
                    { new Guid("00000003-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7970), 3, "LogFinder.Manage", null },
                    { new Guid("00000004-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7971), 4, "TriAndAgilentParser.View", null },
                    { new Guid("00000004-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7973), 4, "TriAndAgilentParser.Manage", null },
                    { new Guid("00000005-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7975), 5, "TechnicalHistory.View", null },
                    { new Guid("00000005-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7990), 5, "TechnicalHistory.Manage", null },
                    { new Guid("00000006-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7992), 6, "KnowledgeBase.View", null },
                    { new Guid("00000006-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7993), 6, "KnowledgeBase.Manage", null },
                    { new Guid("00000007-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7995), 7, "SystemSettings.View", null },
                    { new Guid("00000007-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7996), 7, "SystemSettings.Manage", null },
                    { new Guid("00000008-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7997), 8, "Export.View", null },
                    { new Guid("00000008-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(7999), 8, "Export.Manage", null },
                    { new Guid("00000009-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8000), 9, "Audit.View", null },
                    { new Guid("00000009-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8002), 9, "Audit.Manage", null },
                    { new Guid("0000000a-0001-0000-0000-000000000000"), 1, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8004), 10, "OfflineSync.View", null },
                    { new Guid("0000000a-0005-0000-0000-000000000000"), 5, new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8005), 10, "OfflineSync.Manage", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355"), new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8142), "Acesso operacional tecnico", "Tecnico", null },
                    { new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55"), new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8141), "Acesso total ao sistema", "Administrador", null }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("5a73b376-4e9b-4f79-99e6-2cf1d37d8a8b"), "UI", new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8245), "AutoOpenLastModule", null, "True" },
                    { new Guid("862954d8-ee99-4006-be15-6dce18609376"), "UI", new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8244), "Theme", null, "Dark" },
                    { new Guid("a0cf593a-e9a4-4c54-9e59-f9cd65c50370"), "Updater", new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8243), "AutoCheck", null, "True" },
                    { new Guid("a7ec39fe-48e4-4785-b32b-9a264f71af05"), "Updater", new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8242), "Endpoint", null, "" },
                    { new Guid("e67a95c8-fc91-47a4-a358-f8ee7dbb7b3b"), "Finder", new DateTime(2026, 4, 8, 20, 47, 58, 526, DateTimeKind.Utc).AddTicks(8241), "Directories", null, "" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("00000001-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000002-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000003-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000004-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000005-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000006-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000007-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000008-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000009-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("0000000a-0001-0000-0000-000000000000"), new Guid("2ba5ccdf-6f97-4e5d-9abe-c2dcd4c66355") },
                    { new Guid("00000001-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000001-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000002-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000002-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000003-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000003-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000004-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000004-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000005-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000005-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000006-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000006-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000007-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000007-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000008-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000008-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000009-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("00000009-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("0000000a-0001-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") },
                    { new Guid("0000000a-0005-0000-0000-000000000000"), new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55") }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "MustChangePassword", "PasswordHash", "RoleId", "UpdatedAt", "Username" },
                values: new object[] { new Guid("b8df2f8a-10af-4a6b-9b95-c485e43a1a14"), new DateTime(2026, 4, 8, 20, 47, 58, 536, DateTimeKind.Utc).AddTicks(939), "admin@ict.local", "Administrador do Sistema", true, true, "PBKDF2:bCdgbbfRTcWr6KU/d+yRag==:LHMeoym1w0msk94Xse4zEoXsL3nTRD0VgT40h+TOdw8=", new Guid("dfb4b5b2-4461-44a6-bcc5-6a2ea5a86f55"), null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBaseArticles_Model_TestPhase",
                table: "KnowledgeBaseArticles",
                columns: new[] { "Model", "TestPhase" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module_Action",
                table: "Permissions",
                columns: new[] { "Module", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Category_Key",
                table: "SystemSettings",
                columns: new[] { "Category", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalAnalyses_SerialNumber",
                table: "TechnicalAnalyses",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnowledgeBaseArticles");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TechnicalAnalyses");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}

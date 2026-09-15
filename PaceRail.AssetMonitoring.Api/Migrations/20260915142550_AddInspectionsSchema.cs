using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaceRail.AssetMonitoring.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "RailAssets",
                newName: "AssetTag");

            migrationBuilder.RenameColumn(
                name: "LastInspectedAt",
                table: "RailAssets",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<double>(
                name: "EndChainageMiles",
                table: "RailAssets",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartChainageMiles",
                table: "RailAssets",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "InspectionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RailAssetId = table.Column<int>(type: "integer", nullable: false),
                    InspectorName = table.Column<string>(type: "text", nullable: false),
                    Findings = table.Column<string>(type: "text", nullable: false),
                    RequiresImmediateAction = table.Column<bool>(type: "boolean", nullable: false),
                    InspectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionLogs_RailAssets_RailAssetId",
                        column: x => x.RailAssetId,
                        principalTable: "RailAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLogs_RailAssetId",
                table: "InspectionLogs",
                column: "RailAssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionLogs");

            migrationBuilder.DropColumn(
                name: "EndChainageMiles",
                table: "RailAssets");

            migrationBuilder.DropColumn(
                name: "StartChainageMiles",
                table: "RailAssets");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "RailAssets",
                newName: "LastInspectedAt");

            migrationBuilder.RenameColumn(
                name: "AssetTag",
                table: "RailAssets",
                newName: "Location");
        }
    }
}

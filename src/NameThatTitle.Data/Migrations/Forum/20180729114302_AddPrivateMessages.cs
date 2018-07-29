using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NameThatTitle.Data.Migrations.Forum
{
    public partial class AddPrivateMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrivateMessageId",
                table: "Attachment",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrivateMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FromId = table.Column<int>(nullable: false),
                    ToId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateMessages_UserProfiles_FromId",
                        column: x => x.FromId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrivateMessages_UserProfiles_ToId",
                        column: x => x.ToId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_PrivateMessageId",
                table: "Attachment",
                column: "PrivateMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_FromId",
                table: "PrivateMessages",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_ToId",
                table: "PrivateMessages",
                column: "ToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_PrivateMessages_PrivateMessageId",
                table: "Attachment",
                column: "PrivateMessageId",
                principalTable: "PrivateMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_PrivateMessages_PrivateMessageId",
                table: "Attachment");

            migrationBuilder.DropTable(
                name: "PrivateMessages");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_PrivateMessageId",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "PrivateMessageId",
                table: "Attachment");
        }
    }
}

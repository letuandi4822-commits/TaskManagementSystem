using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskAttachment_Tasks_TaskId",
                table: "TaskAttachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskAttachment",
                table: "TaskAttachment");

            migrationBuilder.RenameTable(
                name: "TaskAttachment",
                newName: "TaskAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_TaskAttachment_TaskId",
                table: "TaskAttachments",
                newName: "IX_TaskAttachments_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskAttachments",
                table: "TaskAttachments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAttachments_Tasks_TaskId",
                table: "TaskAttachments",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskAttachments_Tasks_TaskId",
                table: "TaskAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskAttachments",
                table: "TaskAttachments");

            migrationBuilder.RenameTable(
                name: "TaskAttachments",
                newName: "TaskAttachment");

            migrationBuilder.RenameIndex(
                name: "IX_TaskAttachments_TaskId",
                table: "TaskAttachment",
                newName: "IX_TaskAttachment_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskAttachment",
                table: "TaskAttachment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAttachment_Tasks_TaskId",
                table: "TaskAttachment",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

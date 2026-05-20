using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesWeb.Migrations
{
    /// <inheritdoc />
    public partial class NewStart2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_ParentListId",
                table: "ToDoItems",
                column: "ParentListId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItems_ToDoLists_ParentListId",
                table: "ToDoItems",
                column: "ParentListId",
                principalTable: "ToDoLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItems_ToDoLists_ParentListId",
                table: "ToDoItems");

            migrationBuilder.DropIndex(
                name: "IX_ToDoItems_ParentListId",
                table: "ToDoItems");
        }
    }
}

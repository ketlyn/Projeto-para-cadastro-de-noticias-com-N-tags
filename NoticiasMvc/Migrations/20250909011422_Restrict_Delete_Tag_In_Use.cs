using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoticiasMvc.Migrations
{
    /// <inheritdoc />
    public partial class Restrict_Delete_Tag_In_Use : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoticiaTag_Tag_TagId",
                table: "NoticiaTag");

            migrationBuilder.AddForeignKey(
                name: "FK_NoticiaTag_Tag_TagId",
                table: "NoticiaTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoticiaTag_Tag_TagId",
                table: "NoticiaTag");

            migrationBuilder.AddForeignKey(
                name: "FK_NoticiaTag_Tag_TagId",
                table: "NoticiaTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

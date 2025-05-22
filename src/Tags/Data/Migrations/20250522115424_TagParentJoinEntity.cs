using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagStudio.Tags.Data.Migrations
{
    /// <inheritdoc />
    public partial class TagParentJoinEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagTag");

            migrationBuilder.CreateTable(
                name: "TagParents",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagParents", x => new { x.ChildId, x.ParentId });
                    table.ForeignKey(
                        name: "FK_TagParents_Tags_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagParents_Tags_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagParents_ParentId",
                table: "TagParents",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagParents");

            migrationBuilder.CreateTable(
                name: "TagTag",
                columns: table => new
                {
                    ChildrenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagTag", x => new { x.ChildrenId, x.ParentsId });
                    table.ForeignKey(
                        name: "FK_TagTag_Tags_ChildrenId",
                        column: x => x.ChildrenId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagTag_Tags_ParentsId",
                        column: x => x.ParentsId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagTag_ParentsId",
                table: "TagTag",
                column: "ParentsId");
        }
    }
}

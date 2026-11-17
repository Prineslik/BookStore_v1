using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthorAndJoinTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionModelRoleModel_Permissions_PermissionsId",
                table: "PermissionModelRoleModel");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionModelRoleModel_Roles_RolesId",
                table: "PermissionModelRoleModel");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleModelUserModel_Roles_RolesId",
                table: "RoleModelUserModel");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleModelUserModel_Users_UsersId",
                table: "RoleModelUserModel");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "RoleModelUserModel",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "RoleModelUserModel",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleModelUserModel_UsersId",
                table: "RoleModelUserModel",
                newName: "IX_RoleModelUserModel_UserId");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "PermissionModelRoleModel",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "PermissionsId",
                table: "PermissionModelRoleModel",
                newName: "PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionModelRoleModel_RolesId",
                table: "PermissionModelRoleModel",
                newName: "IX_PermissionModelRoleModel_RoleId");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAuthor",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Books",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Books",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserBook",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook", x => new { x.UserId, x.BookId });
                    table.ForeignKey(
                        name: "FK_UserBook_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBook_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorBook",
                columns: table => new
                {
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorBook", x => new { x.AuthorId, x.BookId });
                    table.ForeignKey(
                        name: "FK_AuthorBook_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorBook_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorBook_BookId",
                table: "AuthorBook",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_UserId",
                table: "Authors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBook_BookId",
                table: "UserBook",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionModelRoleModel_Permissions_PermissionId",
                table: "PermissionModelRoleModel",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionModelRoleModel_Roles_RoleId",
                table: "PermissionModelRoleModel",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModelUserModel_Roles_RoleId",
                table: "RoleModelUserModel",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModelUserModel_Users_UserId",
                table: "RoleModelUserModel",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionModelRoleModel_Permissions_PermissionId",
                table: "PermissionModelRoleModel");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionModelRoleModel_Roles_RoleId",
                table: "PermissionModelRoleModel");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleModelUserModel_Roles_RoleId",
                table: "RoleModelUserModel");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleModelUserModel_Users_UserId",
                table: "RoleModelUserModel");

            migrationBuilder.DropTable(
                name: "AuthorBook");

            migrationBuilder.DropTable(
                name: "UserBook");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsAuthor",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RoleModelUserModel",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "RoleModelUserModel",
                newName: "RolesId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleModelUserModel_UserId",
                table: "RoleModelUserModel",
                newName: "IX_RoleModelUserModel_UsersId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "PermissionModelRoleModel",
                newName: "RolesId");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "PermissionModelRoleModel",
                newName: "PermissionsId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionModelRoleModel_RoleId",
                table: "PermissionModelRoleModel",
                newName: "IX_PermissionModelRoleModel_RolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionModelRoleModel_Permissions_PermissionsId",
                table: "PermissionModelRoleModel",
                column: "PermissionsId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionModelRoleModel_Roles_RolesId",
                table: "PermissionModelRoleModel",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModelUserModel_Roles_RolesId",
                table: "RoleModelUserModel",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModelUserModel_Users_UsersId",
                table: "RoleModelUserModel",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

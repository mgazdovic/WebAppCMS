using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Data.Migrations
{
    public partial class InsertInitialAdminUser : Migration
    {
        const string _adminUserGuid = "7b47dc87-1825-490f-a189-a1454fd9c330";
        const string _adminRoleGuid = "c8234246-b0c3-441c-89bf-01be30f67ad6";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var hasher = new PasswordHasher<AppUser>();
            var passwordHash = hasher.HashPassword(null, "Pass123!");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO AspNetUsers(Id, UserName, NormalizedUserName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, NormalizedEmail, PasswordHash, SecurityStamp, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine("VALUES(");
            sb.AppendLine($"'{_adminUserGuid}'");
            sb.AppendLine(", 'admin@admin.com'");
            sb.AppendLine(", 'ADMIN@ADMIN.COM'");
            sb.AppendLine(", 'admin@admin.com'");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 'ADMIN@ADMIN.COM'");
            sb.AppendLine($", '{passwordHash}'");
            sb.AppendLine(", ''");
            sb.AppendLine($", '{DateTime.Now}'");
            sb.AppendLine($", '{DateTime.Now}'");
            sb.AppendLine($", '{_adminUserGuid}'");
            sb.AppendLine(")");

            string userInsert = sb.ToString();
            string roleInsert = $"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES('{_adminRoleGuid}', 'Admin', 'ADMIN')";
            string userRoleInsert = $"INSERT INTO AspNetUserRoles(UserId, RoleId) VALUES ('{_adminUserGuid}', '{_adminRoleGuid}')";

            // Execute
            migrationBuilder.Sql(userInsert);
            migrationBuilder.Sql(roleInsert);
            migrationBuilder.Sql(userRoleInsert);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId = '{_adminUserGuid}' AND RoleId = '{_adminRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id='{_adminRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id='{_adminUserGuid}'");
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Data.Migrations
{
    public partial class InsertInitialAppUsers : Migration
    {
        const string _adminUserGuid = "7b47dc87-1825-490f-a189-a1454fd9c330";
        const string _adminRoleGuid = "c8234246-b0c3-441c-89bf-01be30f67ad6";

        const string _supervisorUserGuid = "fdd6610c-89b1-47d8-8879-a9749b473a19";
        const string _supervisorRoleGuid = "6f6d1f30-c8ce-43fb-886c-86591a1d05aa";

        const string _clientUserGuid = "2c3612e1-1ebf-41e7-9d5a-f098c6da393a";
        const string _clientRoleGuid = "d702b917-4874-4814-a49f-085b9951cfef";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var hasher = new PasswordHasher<AppUser>();
            var passwordHash = hasher.HashPassword(null, "Pass123!");

            // Admin
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

            migrationBuilder.Sql(userInsert);
            migrationBuilder.Sql(roleInsert);
            migrationBuilder.Sql(userRoleInsert);

            // Supervisor
            sb = new StringBuilder();
            sb.AppendLine("INSERT INTO AspNetUsers(Id, UserName, NormalizedUserName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, NormalizedEmail, PasswordHash, SecurityStamp, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine("VALUES(");
            sb.AppendLine($"'{_supervisorUserGuid}'");
            sb.AppendLine(", 'supervisor@supervisor.com'");
            sb.AppendLine(", 'SUPERVISOR@SUPERVISOR.COM'");
            sb.AppendLine(", 'supervisor@supervisor.com'");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 'SUPERVISOR@SUPERVISOR.COM'");
            sb.AppendLine($", '{passwordHash}'");
            sb.AppendLine(", ''");
            sb.AppendLine($", '{DateTime.Now}'");
            sb.AppendLine($", '{DateTime.Now}'");
            sb.AppendLine($", '{_adminUserGuid}'");
            sb.AppendLine(")");
            migrationBuilder.Sql(sb.ToString());

            // Client
            sb = new StringBuilder();
            sb.AppendLine("INSERT INTO AspNetUsers(Id, UserName, NormalizedUserName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, NormalizedEmail, PasswordHash, SecurityStamp, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine("VALUES(");
            sb.AppendLine($"'{_clientUserGuid}'");
            sb.AppendLine(", 'client@client.com'");
            sb.AppendLine(", 'CLIENT@CLIENT.COM'");
            sb.AppendLine(", 'client@client.com'");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 0");
            sb.AppendLine(", 'CLIENT@CLIENT.COM'");
            sb.AppendLine($", '{passwordHash}'");
            sb.AppendLine(", ''");
            sb.AppendLine($", '{DateTime.Now}'");
            sb.AppendLine($", '{DateTime.Now}'");
            sb.AppendLine($", '{_adminUserGuid}'");
            sb.AppendLine(")");

            migrationBuilder.Sql(sb.ToString());

            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES('{_supervisorRoleGuid}', 'Supervisor', 'SUPERVISOR')");
            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES('{_clientRoleGuid}', 'Client', 'CLIENT')");

            migrationBuilder.Sql($"INSERT INTO AspNetUserRoles(UserId, RoleId) VALUES ('{_supervisorUserGuid}', '{_supervisorRoleGuid}')");
            migrationBuilder.Sql($"INSERT INTO AspNetUserRoles(UserId, RoleId) VALUES ('{_clientUserGuid}', '{_clientRoleGuid}')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Supervisor
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId = '{_supervisorUserGuid}' AND RoleId = '{_supervisorRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id='{_supervisorRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id='{_supervisorUserGuid}'");
            // Client
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId = '{_clientUserGuid}' AND RoleId = '{_clientRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id='{_clientRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id='{_clientUserGuid}'");
            // Admin (deleted last due to ModifiedById foreign key reference constraint)
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId = '{_adminUserGuid}' AND RoleId = '{_adminRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id='{_adminRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id='{_adminUserGuid}'");
        }
    }
}

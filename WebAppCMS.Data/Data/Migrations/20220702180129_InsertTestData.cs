using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Data.Migrations
{
    public partial class InsertTestData : Migration
    {
        const string _supervisorUserGuid = "fdd6610c-89b1-47d8-8879-a9749b473a19";
        const string _supervisorRoleGuid = "6f6d1f30-c8ce-43fb-886c-86591a1d05aa";

        const string _clientUserGuid = "2c3612e1-1ebf-41e7-9d5a-f098c6da393a";
        const string _clientRoleGuid = "d702b917-4874-4814-a49f-085b9951cfef";

        const string _adminUserGuid = "7b47dc87-1825-490f-a189-a1454fd9c330";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // User
            var hasher = new PasswordHasher<AppUser>();
            var passwordHash = hasher.HashPassword(null, "Pass123!");

            StringBuilder sb;
            
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

            // Category & Product
            sb = new StringBuilder();
            sb.AppendLine("DECLARE @CategoryId INT = 0;");

            sb.AppendLine("INSERT INTO Category");
            sb.AppendLine("(Name, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(N'Electronics', '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}');");

            sb.AppendLine("INSERT INTO Category");
            sb.AppendLine("(Name, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(N'Food', '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}');");

            sb.AppendLine("SET @CategoryId = SCOPE_IDENTITY();");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Apple', N'A very nice round fruit.', 5, '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}', 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Banana', N'A very nice yellow fruit.', 6, '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}', 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Kiwi', N'A very nice exotic fruit.', 12, '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}', 1, NULL);");

            sb.AppendLine("INSERT INTO Category");
            sb.AppendLine("(Name, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(N'Drink', '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}');");

            sb.AppendLine("SET @CategoryId = SCOPE_IDENTITY();");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Water', N'The healthiest liquid.', 8, '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}', 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Soda', N'', 8, '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}', 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Coca Cola', N'Too much sugar?', 8, '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}', 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Beer', N'Good for mood ;)', 9, '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}', 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Loza', N'Basically C2H5OH (!)', 90, '{DateTime.Now}', '{DateTime.Now}', N'{_supervisorUserGuid}', 1, NULL);");

            // Order & OrderItem
            sb.AppendLine("DECLARE @ProductId INT = SCOPE_IDENTITY();");
            sb.AppendLine("DECLARE @UserId NVARCHAR(450);");
            sb.AppendLine("SELECT @UserId = [Id] FROM AspNetUsers ORDER BY ModifiedAt DESC;");

            sb.AppendLine("INSERT INTO [Order]");
            sb.AppendLine("(UserId, State, PercentDiscount, PercentTax, DeliveryFee, DeliveryFirstName, DeliveryLastName, DeliveryFullAddress, Message, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(@UserId, 0, 0, 0, 0, N'Miki', N'Milan', N'Ilica 223, Zagreb, Croatia', N'', '{DateTime.Now}', '{DateTime.Now}', @UserId);");

            sb.AppendLine("INSERT INTO [Order]");
            sb.AppendLine("(UserId, State, PercentDiscount, PercentTax, DeliveryFee, DeliveryFirstName, DeliveryLastName, DeliveryFullAddress, Message, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(@UserId, 1, 0, 24, 20, N'Joža', N'Volispit', N'Međimurska 17a, Zagorje, Croatia', N'Nek dojde kam prije jer bum crknul bez pijače', '{DateTime.Now}', '{DateTime.Now}', @UserId);");

            sb.AppendLine("DECLARE @OrderId INT = SCOPE_IDENTITY();");

            sb.AppendLine("INSERT INTO OrderItem");
            sb.AppendLine("(OrderId, ProductId, Quantity, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(@OrderId, @ProductId, 2, '{DateTime.Now}', '{DateTime.Now}', @UserId);");

            sb.AppendLine("INSERT INTO [Order]");
            sb.AppendLine("(UserId, State, PercentDiscount, PercentTax, DeliveryFee, DeliveryFirstName, DeliveryLastName, DeliveryFullAddress, Message, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(@UserId, -1, 0, 0, 0, N'Ante', N'Fulanović', N'Somewhere in the galaxy far, far away...', N'', '{DateTime.Now.AddDays(-1)}', '{DateTime.Now}', N'{_supervisorUserGuid}');");

            migrationBuilder.Sql(sb.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Order & OrderItem (cascade delete)
            migrationBuilder.Sql($"DELETE FROM [Order] WHERE DeliveryLastName IN ('Milan', 'Volispit', 'Fulanović')");

            // Category & Product (cascade delete)
            migrationBuilder.Sql($"DELETE FROM Category WHERE Name IN ('Food', 'Drink', 'Electronics')");

            // User
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId = '{_supervisorUserGuid}' AND RoleId = '{_supervisorRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId = '{_clientUserGuid}' AND RoleId = '{_clientRoleGuid}'");

            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id='{_supervisorUserGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id='{_clientUserGuid}'");

            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id='{_supervisorRoleGuid}'");
            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id='{_clientRoleGuid}'");
        }
    }
}

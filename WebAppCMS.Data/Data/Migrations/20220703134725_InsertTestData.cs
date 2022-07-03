using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text;

namespace WebAppCMS.Data.Migrations
{
    public partial class InsertTestData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            StringBuilder sb = new StringBuilder();

            // User
            sb.AppendLine("DECLARE @UserId_CLIENT NVARCHAR(450);");
            sb.AppendLine("DECLARE @UserId_SUPERVISOR NVARCHAR(450);");
            sb.AppendLine("DECLARE @UserId_ADMIN NVARCHAR(450);");

            sb.AppendLine("SELECT TOP 1 @UserId_ADMIN = U.Id FROM AspNetUsers AS U");
            sb.AppendLine("INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId");
            sb.AppendLine("INNER JOIN AspNetRoles R ON R.Id = UR.RoleId");
            sb.AppendLine("WHERE R.NormalizedName = N'ADMIN'");
            sb.AppendLine("ORDER BY U.CreatedAt;");

            sb.AppendLine("SELECT TOP 1 @UserId_SUPERVISOR = U.Id FROM AspNetUsers AS U");
            sb.AppendLine("INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId");
            sb.AppendLine("INNER JOIN AspNetRoles R ON R.Id = UR.RoleId");
            sb.AppendLine("WHERE R.NormalizedName = N'SUPERVISOR'");
            sb.AppendLine("ORDER BY U.CreatedAt;");

            sb.AppendLine("SELECT TOP 1 @UserId_CLIENT = U.Id FROM AspNetUsers AS U");
            sb.AppendLine("INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId");
            sb.AppendLine("INNER JOIN AspNetRoles R ON R.Id = UR.RoleId");
            sb.AppendLine("WHERE R.NormalizedName = N'CLIENT'");
            sb.AppendLine("ORDER BY U.CreatedAt;");

            sb.AppendLine("SET @UserId_CLIENT = COALESCE(@UserId_CLIENT, @UserId_SUPERVISOR, @UserId_ADMIN);");
            sb.AppendLine("SET @UserId_SUPERVISOR = COALESCE(@UserId_SUPERVISOR, @UserId_ADMIN, @UserId_CLIENT);");
            sb.AppendLine("SET @UserId_ADMIN = COALESCE(@UserId_ADMIN, @UserId_SUPERVISOR, @UserId_CLIENT);");

            // Category & Product
            sb.AppendLine("DECLARE @CategoryId INT = 0;");

            sb.AppendLine("INSERT INTO Category");
            sb.AppendLine("(Name, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(N'Electronics', '{DateTime.Now}', '{DateTime.Now}', @UserId_SUPERVISOR);");

            sb.AppendLine("INSERT INTO Category");
            sb.AppendLine("(Name, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(N'Food', '{DateTime.Now}', '{DateTime.Now}', @UserId_SUPERVISOR);");

            sb.AppendLine("SET @CategoryId = SCOPE_IDENTITY();");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Apple', N'A very nice round fruit.', 5, '{DateTime.Now}', '{DateTime.Now}', @UserId_ADMIN, 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Banana', N'A very nice yellow fruit.', 6, '{DateTime.Now}', '{DateTime.Now}', @UserId_ADMIN, 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Kiwi', N'A very nice exotic fruit.', 12, '{DateTime.Now}', '{DateTime.Now}', @UserId_ADMIN, 1, NULL);");

            sb.AppendLine("INSERT INTO Category");
            sb.AppendLine("(Name, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(N'Drink', '{DateTime.Now}', '{DateTime.Now}', @UserId_SUPERVISOR);");

            sb.AppendLine("SET @CategoryId = SCOPE_IDENTITY();");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Water', N'The healthiest liquid.', 8, '{DateTime.Now}', '{DateTime.Now}', @UserId_ADMIN, 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Soda', N'', 8, '{DateTime.Now}', '{DateTime.Now}', @UserId_ADMIN, 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Coca Cola', N'Too much sugar?', 8, '{DateTime.Now}', '{DateTime.Now}', @UserId_ADMIN, 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Beer', N'Good for mood ;)', 9, '{DateTime.Now}', '{DateTime.Now}', @UserId_ADMIN, 1, NULL);");

            sb.AppendLine("INSERT INTO Product");
            sb.AppendLine("(CategoryId, Name, Description, UnitPrice, CreatedAt, ModifiedAt, ModifiedById, IsAvailable, Image)");
            sb.AppendLine($"VALUES(@CategoryId, N'Loza', N'Basically C2H5OH (!)', 90, '{DateTime.Now}', '{DateTime.Now}', @UserId_ADMIN, 1, NULL);");

            // Order & OrderItem
            sb.AppendLine("DECLARE @ProductId INT = SCOPE_IDENTITY();");

            sb.AppendLine("INSERT INTO [Order]");
            sb.AppendLine("(UserId, State, PercentDiscount, PercentTax, DeliveryFee, DeliveryFirstName, DeliveryLastName, DeliveryFullAddress, Message, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(@UserId_CLIENT, 0, 0, 0, 0, N'Miki', N'Milan', N'Ilica 223, Zagreb, Croatia', N'', '{DateTime.Now}', '{DateTime.Now}', @UserId_SUPERVISOR);");

            sb.AppendLine("INSERT INTO [Order]");
            sb.AppendLine("(UserId, State, PercentDiscount, PercentTax, DeliveryFee, DeliveryFirstName, DeliveryLastName, DeliveryFullAddress, Message, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(@UserId_CLIENT, 1, 0, 24, 20, N'Joža', N'Volispit', N'Međimurska 17a, Zagorje, Croatia', N'Nek dojde kam prije jer bum crknul bez pijače', '{DateTime.Now}', '{DateTime.Now}', @UserId_SUPERVISOR);");

            sb.AppendLine("DECLARE @OrderId INT = SCOPE_IDENTITY();");

            sb.AppendLine("INSERT INTO OrderItem");
            sb.AppendLine("(OrderId, ProductId, Quantity, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(@OrderId, @ProductId, 2, '{DateTime.Now}', '{DateTime.Now}', @UserId_SUPERVISOR);");

            sb.AppendLine("INSERT INTO [Order]");
            sb.AppendLine("(UserId, State, PercentDiscount, PercentTax, DeliveryFee, DeliveryFirstName, DeliveryLastName, DeliveryFullAddress, Message, CreatedAt, ModifiedAt, ModifiedById)");
            sb.AppendLine($"VALUES(@UserId_CLIENT, -1, 0, 0, 0, N'Ante', N'Fulanovic', N'Somewhere in the galaxy far, far away...', N'', '{DateTime.Now.AddDays(-1)}', '{DateTime.Now}', @UserId_SUPERVISOR);");

            // Execute
            migrationBuilder.Sql(sb.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Order & OrderItem (cascade delete)
            migrationBuilder.Sql($"DELETE FROM [Order] WHERE DeliveryLastName IN ('Milan', 'Volispit', 'Fulanovic')");

            // Category & Product (cascade delete)
            migrationBuilder.Sql($"DELETE FROM Category WHERE Name IN ('Food', 'Drink', 'Electronics')");
        }
    }
}

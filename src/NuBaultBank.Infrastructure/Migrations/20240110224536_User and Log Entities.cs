using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuBaultBank.Infrastructure.Migrations;

  public partial class UserandLogEntities : Migration
  {
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.CreateTable(
              name: "Logs",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  ActionStatus = table.Column<int>(type: "int", nullable: false),
                  UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                  Deleted = table.Column<bool>(type: "bit", nullable: false),
                  DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                  CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                  UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                  CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Logs", x => x.Id);
              });

          migrationBuilder.CreateTable(
              name: "Users",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  IsActive = table.Column<bool>(type: "bit", nullable: false),
                  Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  Deleted = table.Column<bool>(type: "bit", nullable: false),
                  DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                  CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                  UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                  CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Users", x => x.Id);
              });
      }

      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "Logs");

          migrationBuilder.DropTable(
              name: "Users");
      }
  }

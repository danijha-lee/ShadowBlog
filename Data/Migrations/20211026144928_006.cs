using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShadowBlog.Data.Migrations
{
    public partial class _006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Blogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Blogs",
                type: "bytea",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BlogPosts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "BlogPosts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abstract",
                table: "BlogPosts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "BlogPosts",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReadyStatus",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "AspNetUsers",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ReadyStatus",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BlogPosts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "BlogPosts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Abstract",
                table: "BlogPosts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60,
                oldNullable: true);
        }
    }
}

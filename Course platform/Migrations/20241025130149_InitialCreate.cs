using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Course_platform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('category_category_id_seq'::regclass)"),
                    category_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("category_pkey", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nickname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    date_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(225)", maxLength: 225, nullable: true),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "regular_user"),
                    verified_degree = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    updated_at = table.Column<DateTimeOffset>(type: "time with time zone", nullable: false, defaultValueSql: "now()"),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    avatar_thumbnail_url = table.Column<string>(type: "text", nullable: true),
                    account_balance = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    chat_id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('chat_chat_id_seq'::regclass)"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    user1_id = table.Column<int>(type: "integer", nullable: true),
                    user2_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chat_pkey", x => x.chat_id);
                    table.ForeignKey(
                        name: "chat_user1_id_fkey",
                        column: x => x.user1_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "chat_user2_id_fkey",
                        column: x => x.user2_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<int>(type: "integer", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("courses_pkey", x => x.course_id);
                    table.ForeignKey(
                        name: "courses_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "followers",
                columns: table => new
                {
                    following_user_id = table.Column<int>(type: "integer", nullable: false),
                    followed_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("followers_pkey", x => new { x.following_user_id, x.followed_user_id });
                    table.ForeignKey(
                        name: "followers_followed_user_id_fkey",
                        column: x => x.followed_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "followers_following_user_id_fkey",
                        column: x => x.following_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('message_message_id_seq'::regclass)"),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    message_text = table.Column<string>(type: "text", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    chat_id = table.Column<int>(type: "integer", nullable: true),
                    read = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("message_pkey", x => x.message_id);
                    table.ForeignKey(
                        name: "message_chat_id_fkey",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "chat_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "message_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_category_map",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("course_category_map_pkey", x => new { x.course_id, x.category_id });
                    table.ForeignKey(
                        name: "course_category_map_category_id_fkey",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "course_category_map_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    module_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: true),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_complited = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("modules_pkey", x => x.module_id);
                    table.ForeignKey(
                        name: "modules_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    rating_id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('rating_rating_id_seq'::regclass)"),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    course_id = table.Column<int>(type: "integer", nullable: true),
                    rating = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("rating_pkey", x => x.rating_id);
                    table.ForeignKey(
                        name: "rating_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "rating_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    course_id = table.Column<int>(type: "integer", nullable: true),
                    platform_fee = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transactions_pkey", x => x.transaction_id);
                    table.ForeignKey(
                        name: "transactions_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "transactions_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_course_progress",
                columns: table => new
                {
                    progress_id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('usercourseprogress_progress_id_seq'::regclass)"),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    course_id = table.Column<int>(type: "integer", nullable: true),
                    state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'not started'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("usercourseprogress_pkey", x => x.progress_id);
                    table.ForeignKey(
                        name: "usercourseprogress_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "usercourseprogress_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    lesson_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    is_complited = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lessons_pkey", x => x.lesson_id);
                    table.ForeignKey(
                        name: "lessons_module_id_fkey",
                        column: x => x.module_id,
                        principalTable: "modules",
                        principalColumn: "module_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "chat_user1_id_user2_id_key",
                table: "chats",
                columns: new[] { "user1_id", "user2_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chats_user2_id",
                table: "chats",
                column: "user2_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_category_map_category_id",
                table: "course_category_map",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_courses_user_id",
                table: "courses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_followers_followed_user_id",
                table: "followers",
                column: "followed_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_module_id",
                table: "lessons",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_chat_id",
                table: "messages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_user_id",
                table: "messages",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_modules_course_id",
                table: "modules",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_course_id",
                table: "ratings",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_user_id",
                table: "ratings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_course_id",
                table: "transactions",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id",
                table: "transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_course_progress_course_id",
                table: "user_course_progress",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "usercourseprogress_user_id_course_id_key",
                table: "user_course_progress",
                columns: new[] { "user_id", "course_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "unique_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_category_map");

            migrationBuilder.DropTable(
                name: "followers");

            migrationBuilder.DropTable(
                name: "lessons");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "user_course_progress");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "modules");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

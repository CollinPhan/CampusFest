using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clubs_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampusId = table.Column<int>(type: "int", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Accounts_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Accounts_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PosterURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    club = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Clubs_club",
                        column: x => x.club,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidAccount = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Accounts_ValidAccount",
                        column: x => x.ValidAccount,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckinTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Campuses",
                columns: new[] { "Id", "Address", "Description", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "", "Chính thức thành lập ngày 8/9/2006 theo Quyết định của Thủ tướng Chính phủ, Trường Đại học FPT trở thành trường đại học đầu tiên của Việt Nam do một doanh nghiệp đứng ra thành lập với 100% vốn đầu tư từ Tập đoàn FPT.\r\n\r\nSự khác biệt của Trường Đại học FPT so với các trường đại học khác là đào tạo theo hình thức liên kết chặt chẽ với các doanh nghiệp, gắn đào tạo với thực tiễn, với nghiên cứu – triển khai và các công nghệ hiện đại nhất. Triết lý và phương pháp giáo dục hiện đại; Đào tạo con người toàn diện, hài hòa; Chương trình luôn được cập nhật và tuân thủ các chuẩn công nghệ quốc tế; Đặc biệt chú trọng kỹ năng ngoại ngữ; Tăng cường đào tạo quy trình tổ chức sản xuất, kỹ năng làm việc theo nhóm và các kỹ năng cá nhân khác là những điểm sẽ đảm bảo cho sinh viên tốt nghiệp có những cơ hội việc làm tốt nhất sau khi ra trường.", "", "FPT University", "" },
                    { 2, "", "Trường Đại học Bách khoa - ĐHQG-HCM  là một trường thành viên của hệ thống Đại học Quốc gia TP. Hồ Chí Minh. Tiền thân của Trường là Trung tâm Quốc gia Kỹ thuật được thành lập vào năm 1957. Hiện nay, Trường ĐH Bách Khoa là trung tâm đào tạo, nghiên cứu khoa học và chuyển giao công nghệ lớn nhất các tỉnh phía Nam và là trường đại học kỹ thuật quan trọng của cả nước.", "", "Đại học Bách Khoa Thành phố Hồ Chí Minh", "" },
                    { 3, "", "Trường ĐH Nguyễn Tất Thành là “trường đại học đổi mới sáng tạo” đáp ứng nhu cầu giáo dục đại học đại chúng thông qua việc tạo lập một môi trường học tập tích cực và trải nghiệm thực tiễn cho mọi sinh viên trang bị cho người học năng lực tự học tinh thần sáng tạo khởi nghiệp có trách nhiệm với cộng đồng hội nhập với khu vực và toàn cầu.\r\n\r\nTriết lý giáo dục của Nhà trường: \r\n\r\nTHỰC HỌC – THỰC HÀNH – THỰC DANH – THỰC NGHIỆP ", "", "Đại học Nguyễn Tất Thành", "" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "SystemAdmin" },
                    { 2, "ClubEventOrganizer" },
                    { 3, "ClubEventStaff" },
                    { 4, "Visitor" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Avatar", "CampusId", "ClubId", "CreatedTime", "Email", "Fullname", "IsDeleted", "IsVerified", "LastUpdatedTime", "Password", "Phone", "RoleId", "Username" },
                values: new object[] { new Guid("4cf5c11b-8f8a-4959-ad93-d5558fb1b6e7"), "", null, null, new DateTime(2024, 11, 1, 10, 43, 22, 584, DateTimeKind.Local).AddTicks(8858), "[Your email goes here]", "Collin", false, true, new DateTime(2024, 11, 1, 3, 43, 22, 584, DateTimeKind.Utc).AddTicks(8880), "BCE37EEE9DFA8D910FA103096F49A341B741FA698BF07E6771C1BF9653B38A80370465623389C702CBC686C760C7D377A394299AA39C2EFCFEFE25A58DFE3400948DEC030F350DA467E5DC0A690183F326980470AB1E560CB35784F51BCB2007AE9CE57BFB55D5C58E3DC3D0F134BE3AD114DEA64BA57C2938391AC8AC17E22C746397DF1F2EA0498FCB0B3FA4AFFB04C4157EF371DCD01D34CA8725DF295DEA3AB0F91AA83C63A7327394E013F795796AE54ABBB88280848C4C18CA86A0350CC65D2B0427678268C36BBBE07E4B0539C4DF98BC79B14CC21022F5BC36CFB4D1B135E3F9D14741850696B0193347936D56A135559AD1C989DFBAB39F71A9B451", "", 1, "SystemAdmin" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CampusId",
                table: "Accounts",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ClubId",
                table: "Accounts",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_CampusId",
                table: "Clubs",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_AccountId",
                table: "EventRegistrations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistrations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_club",
                table: "Events",
                column: "club");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_ValidAccount",
                table: "Tokens",
                column: "ValidAccount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Campuses");
        }
    }
}

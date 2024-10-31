using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Campuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}

using Backend.Cores.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructures.Data
{
    public class CampusFestDbContext: DbContext
    {
        public CampusFestDbContext(DbContextOptions options): base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<ClubEventStaff> ClubEventStaffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(table =>
            {
                table.HasData(
                    new Role {Id = 1, Name="SystemAdmin" },
                    new Role {Id = 2, Name="ClubEventOrganizer"},
                    new Role {Id = 3, Name = "ClubEventStaff" },
                    new Role {Id = 4, Name = "Visitor"}
                );
            });

            modelBuilder.Entity<Campus>(table =>
            {
                table.HasData(
                    new Campus {Id = 1, Name = "FPT University", Description = "Chính thức thành lập ngày 8/9/2006 theo Quyết định của Thủ tướng Chính phủ, Trường Đại học FPT trở thành trường đại học đầu tiên của Việt Nam do một doanh nghiệp đứng ra thành lập với 100% vốn đầu tư từ Tập đoàn FPT.\r\n\r\nSự khác biệt của Trường Đại học FPT so với các trường đại học khác là đào tạo theo hình thức liên kết chặt chẽ với các doanh nghiệp, gắn đào tạo với thực tiễn, với nghiên cứu – triển khai và các công nghệ hiện đại nhất. Triết lý và phương pháp giáo dục hiện đại; Đào tạo con người toàn diện, hài hòa; Chương trình luôn được cập nhật và tuân thủ các chuẩn công nghệ quốc tế; Đặc biệt chú trọng kỹ năng ngoại ngữ; Tăng cường đào tạo quy trình tổ chức sản xuất, kỹ năng làm việc theo nhóm và các kỹ năng cá nhân khác là những điểm sẽ đảm bảo cho sinh viên tốt nghiệp có những cơ hội việc làm tốt nhất sau khi ra trường." },
                    new Campus {Id = 2, Name = "Đại học Bách Khoa Thành phố Hồ Chí Minh", Description = "Trường Đại học Bách khoa - ĐHQG-HCM  là một trường thành viên của hệ thống Đại học Quốc gia TP. Hồ Chí Minh. Tiền thân của Trường là Trung tâm Quốc gia Kỹ thuật được thành lập vào năm 1957. Hiện nay, Trường ĐH Bách Khoa là trung tâm đào tạo, nghiên cứu khoa học và chuyển giao công nghệ lớn nhất các tỉnh phía Nam và là trường đại học kỹ thuật quan trọng của cả nước." },
                    new Campus {Id = 3, Name = "Đại học Nguyễn Tất Thành", Description = "Trường ĐH Nguyễn Tất Thành là “trường đại học đổi mới sáng tạo” đáp ứng nhu cầu giáo dục đại học đại chúng thông qua việc tạo lập một môi trường học tập tích cực và trải nghiệm thực tiễn cho mọi sinh viên trang bị cho người học năng lực tự học tinh thần sáng tạo khởi nghiệp có trách nhiệm với cộng đồng hội nhập với khu vực và toàn cầu.\r\n\r\nTriết lý giáo dục của Nhà trường: \r\n\r\nTHỰC HỌC – THỰC HÀNH – THỰC DANH – THỰC NGHIỆP " }

                );
            });
        }
    }
}

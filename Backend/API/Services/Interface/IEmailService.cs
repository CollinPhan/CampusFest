namespace Backend.API.Services.Interface
{
    public interface IEmailService: IDisposable
    {
        Task SendEmailAsync(string target, string subject, string body);

        Task SendEmailAsync(IEnumerable<string> target_list, string title, string body);
    }
}

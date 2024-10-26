namespace Backend.API.Services.Interface
{
    public interface EmailService: IDisposable
    {
        Task SendEmailAsync(string title, string body, string email, FileStream file);
    }
}

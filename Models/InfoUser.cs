namespace BotTelegram.Models;

public class InfoUser
{
    public int Id { get; set; }
    public int InsertUserId { get; set; }
    public int AddedUserId { get; set; }
    public long ChatId { get; set; }
    public DateTime? LeftUserDate { get; set; }
}
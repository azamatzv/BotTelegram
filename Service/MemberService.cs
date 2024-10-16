using BotTelegram.Data;
using BotTelegram.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTelegram;

public static class MemberService
{
    public static async Task HandleTextMessage(ITelegramBotClient botClient, Update update, BotContext dbContext, CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;
        var userId = update.Message.From.Id;
        var text = update.Message.Text;

        var chatMember = await botClient.GetChatMemberAsync(chatId, (long)botClient.BotId, cancellationToken);

        if (text.StartsWith("/mymember"))
        {
            int addedMembersCount = await GetUserAddedMembersCount(userId, chatId, dbContext);
            await botClient.SendTextMessageAsync(chatId, $"{update.Message.From.FirstName} \n 🔹Siz {addedMembersCount} ta odam qo'shgansiz!", cancellationToken: cancellationToken);
        }

        else if (update.Message.ReplyToMessage != null && text.StartsWith("/yourmember"))
        {
            var repliedUserId = update.Message.ReplyToMessage.From.Id;
            int addedMembersCount = await GetUserAddedMembersCount(repliedUserId, chatId, dbContext);
            await botClient.SendTextMessageAsync(chatId, $"{update.Message.From.FirstName} \n 🔹 {addedMembersCount} ta odam qo'shgan!", cancellationToken: cancellationToken);
        }

        else if (text.StartsWith("/start"))
        {
            await botClient.SendTextMessageAsync(chatId,
                "Salom, hurmatli foydalanuvchi! 👋\r\n\r\n" +
                "Bizning botga xush kelibsiz! 🎉\r\n\r\n", cancellationToken: cancellationToken);
        }

        else if (text.StartsWith("/complaint"))
        {
            string shikoyatMatni = update.Message.Text.Substring("/complaint".Length).Trim();
            if (!string.IsNullOrEmpty(shikoyatMatni))
            {
                await botClient.SendTextMessageAsync(
                    chatId: 1204242620,
                    text: $"Yangi shikoyat:\n\nFoydalanuvchi: {update.Message.From.FirstName} {update.Message.From.LastName} @{update.Message.From.Username}\n" +
                          $"Foydalanuvchi ID: {update.Message.From.Id}\n\n" +
                          $"Shikoyat matni: {shikoyatMatni}"
                );
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sizning shikoyatingiz qabul qilindi. Rahmat!");
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Iltimos, shikoyat matnini kiriting. Masalan: /complaint Mening shikoyatim");
            }
        }

        else if (text.StartsWith("/admins"))
        {
            var admins = await botClient.GetChatAdministratorsAsync(chatId, cancellationToken);
            foreach (var admin in admins)
            {
                await botClient.SendTextMessageAsync(chatId, $"Hurmatli admin @{admin.User.Username}, sizga murojaat bor.", cancellationToken: cancellationToken);
            }
        }
    }


    public static async Task<int> GetUserAddedMembersCount(long userId, long chatId, BotContext dbContext)
    {
        var count = await dbContext.Users.Where(u => u.InsertUserId == userId && u.LeftUserDate == null && u.ChatId == chatId).CountAsync();

        return count;
    }

    public static async Task InsertNewMember(long insertUserId, long addedUserId, long chatId, BotContext dbContext)
    {
        var newUser = new InfoUser
        {
            InsertUserId = (int)insertUserId,
            AddedUserId = (int)addedUserId,
            ChatId = chatId,
            LeftUserDate = null
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();
    }

    public static async Task UpdateUserLeftDate(long userId, long chatId, BotContext dbContext)
    {
        var user = await dbContext.Users
            .Where(u => u.AddedUserId == userId && u.ChatId == chatId && u.LeftUserDate == null)
            .FirstOrDefaultAsync();

        if (user != null)
        {
            user.LeftUserDate = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }
    }
}
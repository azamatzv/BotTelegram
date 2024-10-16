using BotTelegram.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotTelegram;

public static class BotHandlers
{
    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message == null) return;

        var chatId = update.Message.Chat.Id;

        using (var dbContext = new BotContext())
        {
            switch (update.Message.Type)
            {
                case MessageType.Text:
                    // Бу рекламани о'чирадиган қисм. Ишлатолмаганман.
                    //await AdFilterService.RemoveAdvertisementOrUrlAndBanUser(botClient, chatId, update.Message.From, update.Message.MessageId, update.Message.Text, cancellationToken);
                    
                    await MemberService.HandleTextMessage(botClient, update, dbContext, cancellationToken);
                    break;

                case MessageType.ChatMembersAdded:
                    foreach (var newMember in update.Message.NewChatMembers)
                    {
                        await MemberService.InsertNewMember(update.Message.From.Id, newMember.Id, chatId, dbContext);
                    }
                    break;

                case MessageType.ChatMemberLeft:
                    var leftUserId = update.Message.LeftChatMember.Id;
                    await MemberService.UpdateUserLeftDate(leftUserId, chatId, dbContext);
                    break;

                default:
                    break;
            }
        }
    }

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            Telegram.Bot.Exceptions.ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}
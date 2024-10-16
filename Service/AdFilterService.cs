using Telegram.Bot.Types;
using Telegram.Bot;

namespace BotTelegram.Services
{
    public class AdFilterService
    {
        public static bool IsAdvertisementOrUrl(string? message)
        {
            // Реклама о'чирадиган қисми ишламаябди, анча харакат қилиб ко'рдим.
            // Реклама о'чирадиган жойга келса Бот 400 АPI ERROR бериб бот умуман ишламай қолябди, бу қисм о'хшамади.

            if (string.IsNullOrEmpty(message)) return false;

            string[] urlPatterns = { "http://", "https://", "www.", ".com", ".net", ".org" };

            foreach (var pattern in urlPatterns)
            {
                if (message.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            string[] advertisementKeywords = { "reklama", "rekl", "promocod", "rek" };

            foreach (var keyword in advertisementKeywords)
            {
                if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static async Task RemoveAdvertisementOrUrlAndBanUser(ITelegramBotClient botClient, long chatId, User user, int messageId, string? messageText, CancellationToken cancellationToken)
        {
            if (IsAdvertisementOrUrl(messageText))
            {
                await botClient.DeleteMessageAsync(chatId, messageId, cancellationToken);
                Console.WriteLine("Reklama yoki URL xabar o'chirildi.");

                await BanUserAsync(botClient, chatId, user, cancellationToken);
            }
        }

        private static async Task BanUserAsync(ITelegramBotClient botClient, long chatId, User user, CancellationToken cancellationToken)
        {
            await botClient.BanChatMemberAsync(chatId, user.Id, DateTime.UtcNow.AddSeconds(30), cancellationToken:cancellationToken);

            var username = !string.IsNullOrEmpty(user.Username) ? $"@{user.Username}" : $"{user.FirstName} {user.LastName}";
            await botClient.SendTextMessageAsync(chatId, $"{username} 30 soniyaga ban qilindi, chunki u reklama tashladi.", cancellationToken: cancellationToken);

            Console.WriteLine($"{username} 30 soniyaga ban qilindi.");

            await botClient.UnbanChatMemberAsync(chatId, user.Id , cancellationToken: cancellationToken);

            Console.WriteLine("Bandan chiqdi");
        }
    }
}
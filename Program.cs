using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace BotTelegram;

public class Program
{
    private static ITelegramBotClient? botClient;

    static async Task Main(string[] args)
    {
        botClient = new TelegramBotClient("7609952653:AAGc9-ap4Cag4rYYzBRQ8Q595l2HRU9AKk8");

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Bot {me.Username} ishga tushdi.");

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            BotHandlers.HandleUpdateAsync,
            BotHandlers.HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        Console.WriteLine("Bot to'xtash uchun tugma bosing.");
        Console.ReadLine();

        cancellationTokenSource.Cancel();
    }
}
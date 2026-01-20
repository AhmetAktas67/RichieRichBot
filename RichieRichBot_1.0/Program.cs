
using RichieRichBot.Core;

class Program
{
    static async Task Main()
    {
        Console.Title = "RichieRichBot";

        var bot = new Bot();
        await bot.StartAsync();

        await Task.Delay(-1); // Bot am Leben halten
    }
}




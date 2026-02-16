
using RichieRichBot.Core;

class Program
{
    static async Task Main()
    {
        Console.Title = "RichieRichBot";

        var bot = new Bot();
        await bot.StartAsync();

        
    }
}




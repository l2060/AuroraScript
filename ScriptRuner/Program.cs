// See https://aka.ms/new-console-template for more information
using AuroraScript;

//rsTest.Run();


public class Program
{



    public static async Task Main()
    {

        var b = 252;

        var value1 = b | 0x01;
        var value2 = b | 0x02;
        var value3 = b | 0x03;
        int lastTwoBits1 = value2 & 0x03;  // 只保留最低2位 0 1 2 3
        int restoredB3 = value3 & ~0x03; // 清除最低两位  0 - 252






        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./tests/" });
        //try
        //{
        await engine.BuildAsync("./unit.as");
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"[Error]: {ex.Message}");
        //}


        Console.WriteLine("=====================================================================================");
        Console.ReadKey();
    }

}
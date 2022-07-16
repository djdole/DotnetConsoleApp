
namespace ConsoleApp
{
    using Net.DJDole.Settings;

    class Program
    {
        public static async Task Main()
        {
            ServiceSettings settings = new ServiceSettings(name: "ConsoleApp", path: "Settings.json");
            settings.Start();
            Console.WriteLine(settings.Values.Message);
        }
    }
}

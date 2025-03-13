using HmxLabs.TechTest.Models;
using HmxLabs.TechTest.RiskSystem;

namespace HmxLabs.TechTest.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args_)
        {
            var tradeLoader = new SerialTradeLoader();

            var allTrades = tradeLoader.LoadTrades();
            var results = new ScalarResults();
            // var pricer = new SerialPricer();
            var pricer = new ParallelPricer();
            pricer.Price(allTrades, results);

            var screenPrinter = new ScreenResultPrinter();
            screenPrinter.PrintResults(results);

            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }
    }
}
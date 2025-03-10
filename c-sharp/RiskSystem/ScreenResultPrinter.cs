using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class ScreenResultPrinter
    {
        public void PrintResults(ScalarResults results_)
        {
            foreach (var result in results_)
            {
                // Write code here to print out the results such that we have : 
                // TradeID : Result : Error
                // If there is no result then the output should be :
                // TradeID : Error
                // If there is no error the output should be :
                // TradeID : Result
                Console.Write(result.TradeId);
                if (result.Result.HasValue)
                {
                    Console.Write($" : {result.Result.Value}");
                }
                if (!string.IsNullOrWhiteSpace(result.Error))
                {
                    Console.Write($" : {result.Error}");
                }
                Console.WriteLine();
            }
        }
    }
}
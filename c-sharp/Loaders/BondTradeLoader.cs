using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class BondTradeLoader : ITradeLoader
    {
        private const char Seperator = ',';

        public IEnumerable<ITrade> LoadTrades()
        {
            return LoadTradesFromFile(DataFile);

        }

        public string? DataFile { get; set; }

        private ITrade CreateTradeFromLine(string line_)
        {
            var items = line_.Split(new[] { Seperator });
            string bondType = items[0];
            ITrade trade;

            switch (bondType)
            {
                case BondTrade.GovBondTradeType:
                    trade = new BondTrade(items[6], BondTrade.GovBondTradeType);
                    break;
                case BondTrade.CorpBondTradeType:
                    trade = new BondTrade(items[6], BondTrade.CorpBondTradeType);
                    break;
                default:
                    trade = new BondTrade(items[6], BondTrade.CorpBondTradeType);
                    break;
            }

            trade.TradeDate = DateTime.Parse(items[1]);
            trade.Instrument = items[2];
            trade.Counterparty = items[3];
            trade.Notional = Double.Parse(items[4]);
            trade.Rate = Double.Parse(items[5]);

            return trade;
        }

        private IEnumerable<ITrade> LoadTradesFromFile(string? filename_)
        {
            if (null == filename_)
                throw new ArgumentNullException(nameof(filename_));

            var stream = new StreamReader(filename_);

            using (stream)
            {
                var lineCount = 0;
                while (!stream.EndOfStream)
                {
                    if (0 == lineCount)
                    {
                        stream.ReadLine();
                    }
                    else
                    {
                        yield return CreateTradeFromLine(stream.ReadLine()!);
                    }
                    lineCount++;
                }
            }
        }
    }
}

using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class FxTradeLoader : ITradeLoader
    {
        private const char Seperator = '¬';

        public IEnumerable<ITrade> LoadTrades()
        {
            var tradeList = new FxTradeList();
            LoadTradesFromFile(DataFile, tradeList);

            return tradeList;
        }

        public string? DataFile { get; set; }

        protected ITrade CreateTradeFromLine(string line_)
        {
            var items = line_.Split(new[] { Seperator });

            if (items.Length != 9)
            {
                throw new InvalidDataException("Invalid number of fields in trade data");
            }

            string tradeType = items[0];
            FxTrade trade;

            switch (tradeType)
            {
                case FxTrade.FxSpotTradeType:
                    trade = new FxTrade(items[8], FxTrade.FxSpotTradeType);
                    break;
                case FxTrade.FxForwardTradeType:
                    trade = new FxTrade(items[8], FxTrade.FxForwardTradeType);
                    break;
                default:
                    trade = new FxTrade(items[8], FxTrade.FxForwardTradeType);
                    break;
            }

            trade.TradeDate = DateTime.Parse(items[1]);
            trade.Instrument = $"{items[2]}{items[3]}";
            trade.Counterparty = items[7];
            trade.Notional = Double.Parse(items[4]);
            trade.Rate = Double.Parse(items[5]);
            trade.ValueDate = DateTime.Parse(items[6]);

            return trade;
        }

        private void LoadTradesFromFile(string? filename_, FxTradeList tradeList_)
        {
            if (null == filename_)
                throw new ArgumentNullException(nameof(filename_));

            var stream = new StreamReader(filename_);

            using (stream)
            {
                var lineCount = 0;
                while (!stream.EndOfStream)
                {
                    if (2 > lineCount)
                    {
                        stream.ReadLine();
                    }
                    else
                    {
                        try
                        {
                            tradeList_.Add(CreateTradeFromLine(stream.ReadLine()!));
                        }
                        catch (InvalidDataException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    lineCount++;
                }
            }
        }
    }
}

namespace HmxLabs.TechTest.Models
{
    public class BondTrade : BaseTrade
    {
        public override string TradeType { get; }
        public const string CorpBondTradeType = "CorpBond";
        public const string GovBondTradeType = "GovBond";

        public BondTrade(string tradeId_, string tradeType_)
        {
            if (string.IsNullOrWhiteSpace(tradeId_))
            {
                throw new ArgumentException("A valid non null, non empty trade ID must be provided");
            }

            if (tradeType_ != GovBondTradeType && tradeType_ != CorpBondTradeType)
            {
                throw new ArgumentException($"Trade type must be either {GovBondTradeType} or {CorpBondTradeType}");
            }

            TradeId = tradeId_;
            TradeType = tradeType_;
        }


    }
}


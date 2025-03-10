namespace HmxLabs.TechTest.Models
{
    public class BondTrade : BaseTrade
    {
        private readonly string _tradeType;

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
            _tradeType = tradeType_;
        }

        public const string GovBondTradeType = "GovBond";
        public const string CorpBondTradeType = "CorpBond";

        public override string TradeType => _tradeType;
    }
}


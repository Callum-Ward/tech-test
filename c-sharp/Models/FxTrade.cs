using System;

namespace HmxLabs.TechTest.Models
{
    public class FxTrade : BaseTrade
    {
        public const string FxSpotTradeType = "FxSpot";
        public const string FxForwardTradeType = "FxFwd";
        public override string TradeType { get; }
        public DateTime ValueDate { get; set; }


        public FxTrade(string tradeId_, string tradeType_)
        {
            if (string.IsNullOrWhiteSpace(tradeId_))
            {
                throw new ArgumentException("A valid non null, non empty trade ID must be provided");
            }

            if (tradeType_ != FxForwardTradeType && tradeType_ != FxSpotTradeType)
            {
                throw new ArgumentException($"Trade type must be either {FxSpotTradeType} or {FxForwardTradeType}");
            }

            TradeId = tradeId_;
            TradeType = tradeType_;
        }

    }
}


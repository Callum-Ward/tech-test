using System.Collections;
using System.Collections.Generic;

namespace HmxLabs.TechTest.Models
{
    public class ScalarResults : IScalarResultReceiver, IEnumerable<ScalarResult>
    {
        public ScalarResult? this[string tradeId_]
        {
            get
            {
                if (!ContainsTrade(tradeId_))
                {
                    return null;
                }

                    double? priceResult = null;
                    string? error = null;
                    if (_results.ContainsKey(tradeId_))
                    {
                        priceResult = _results[tradeId_];
                    }
                    if (_errors.ContainsKey(tradeId_))
                    {
                        error = _errors[tradeId_];
                    }

                return new ScalarResult(tradeId_, priceResult, error);
            }
        }

        public bool ContainsTrade(string tradeId_)
        {
            if (_results.ContainsKey(tradeId_) || _errors.ContainsKey(tradeId_))
            {
                return true;
            }

            return false;
        }

        public void AddResult(string tradeId_, double result_)
        {
            _results.Add(tradeId_, result_);
        }

        public void AddError(string tradeId_, string? error_)
        {
            _errors.Add(tradeId_, error_);
        }

        public IEnumerator<ScalarResult> GetEnumerator()
        {
            // Get all unique trade IDs from both dictionaries
            var allTradeIds = new HashSet<string>(_results.Keys);
            allTradeIds.UnionWith(_errors.Keys);

            // Create a ScalarResult for each unique trade ID
            foreach (var tradeId in allTradeIds)
            {
                double? priceResult = null;
                string? error = null;

                if (_results.ContainsKey(tradeId))
                {
                    priceResult = _results[tradeId];
                }

                if (_errors.ContainsKey(tradeId))
                {
                    error = _errors[tradeId];
                }

                yield return new ScalarResult(tradeId, priceResult, error);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        private readonly Dictionary<string, double> _results = new Dictionary<string, double>();
        private readonly Dictionary<string, string?> _errors = new Dictionary<string, string?>();
    }
}
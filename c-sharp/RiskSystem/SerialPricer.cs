using System;
using System.Collections.Generic;
using System.Reflection;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class SerialPricer
    {
        private readonly Dictionary<string, IPricingEngine> _pricers = new Dictionary<string, IPricingEngine>();

        public SerialPricer()
        {
            LoadPricers(); // Load pricers at initialization
        }

        public void Price(IEnumerable<IEnumerable<ITrade>> tradeContainters_, IScalarResultReceiver resultReceiver_)
        {

            foreach (var tradeContainter in tradeContainters_)
            {
                foreach (var trade in tradeContainter)
                {
                    if (!_pricers.ContainsKey(trade.TradeType))
                    {
                        resultReceiver_.AddError(trade.TradeId, "No Pricing Engines available for this trade type");
                        continue;
                    }

                    var pricer = _pricers[trade.TradeType];
                    pricer.Price(trade, resultReceiver_);
                }
            }
        }

        private void LoadPricers()
        {
            var pricingConfigLoader = new PricingConfigLoader { ConfigFile = @".\PricingConfig\PricingEngines.xml" };
            var pricerConfig = pricingConfigLoader.LoadConfig();

            foreach (var configItem in pricerConfig)
            {
                if (string.IsNullOrEmpty(configItem.TradeType))
                {
                    throw new InvalidDataException("Trade type not specified in config");
                }

                if (string.IsNullOrEmpty(configItem.Assembly) || string.IsNullOrEmpty(configItem.TypeName))
                {
                    throw new InvalidDataException($"Assembly or pricing engine not specified for trade type: {configItem.TradeType}");
                }

                try
                {
                    var shortAssembly = configItem.Assembly.Split('.').Last();
                    var shortAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{shortAssembly}.dll");

                    Assembly assembly = Assembly.LoadFrom(shortAssemblyPath);

                    // Get the type from the assembly - use null-coalescing operator to throw if type not found
                    Type engineType = assembly.GetType(configItem.TypeName)
                        ?? throw new InvalidOperationException($"Could not load type: {configItem.TypeName} from assembly: {configItem.Assembly}");

                    // Create an instance of the pricing engine
                    IPricingEngine pricingEngine = (IPricingEngine)(Activator.CreateInstance(engineType)
                        ?? throw new InvalidOperationException($"Failed to create instance of type: {configItem.TypeName}"));

                    _pricers.Add(configItem.TradeType, pricingEngine);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to instantiate pricing engine for trade type: {configItem.TradeType}. Error: {ex.Message}", ex);
                }
            }
        }
    }
}
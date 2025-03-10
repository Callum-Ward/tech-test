using System.Xml.Linq;


namespace HmxLabs.TechTest.RiskSystem
{
    public class PricingConfigLoader
    {
        public string? ConfigFile { get; set; }

        public PricingEngineConfig LoadConfig()
        {
            if (string.IsNullOrEmpty(ConfigFile)) {
                throw new InvalidOperationException("ConfigFile property is not set.");
            }

            var config = new PricingEngineConfig();
            var doc = XDocument.Load(ConfigFile);

            foreach (var engineElement in doc.Descendants("Engine"))
            {
                var configItem = new PricingEngineConfigItem
                {
                    TradeType = engineElement.Attribute("tradeType")?.Value,
                    Assembly = engineElement.Attribute("assembly")?.Value,
                    TypeName = engineElement.Attribute("pricingEngine")?.Value
                };
                config.Add(configItem);
            }
            return config;
        }
    }
}
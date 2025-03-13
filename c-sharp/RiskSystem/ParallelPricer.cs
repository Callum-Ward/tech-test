using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    // Parallel implementation of the pricing engine that processes trades concurrently
    // using a ThreadPool for better scalability with complex trade types.
    public class ParallelPricer
    {
        private readonly Dictionary<string, IPricingEngine> _pricers = new Dictionary<string, IPricingEngine>();
        private readonly int _maxDegreeOfParallelism;

        // Creates a new instance of the ParallelPricer with default number of parallel tasks.
        public ParallelPricer() : this(Environment.ProcessorCount) { }

        // Creates a new instance of the ParallelPricer with a specified number of parallel tasks.
        public ParallelPricer(int maxDegreeOfParallelism)
        {
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            LoadPricers();
        }

        // Prices all provided trades in parallel, using a thread pool to run a number
        // of pricing calculations concurrently. Returns only when all trades are priced.
        public void Price(IEnumerable<IEnumerable<ITrade>> tradeContainers_, IScalarResultReceiver resultReceiver_)
        {
            // Prepare a thread-safe collection of trades to process
            var tradesToProcess = new ConcurrentQueue<ITrade>();

            // Enqueue all trades 
            foreach (var tradeContainer in tradeContainers_)
            {
                foreach (var trade in tradeContainer)
                {
                    tradesToProcess.Enqueue(trade);
                }
            }

            // Process trades in parallel with a limit on the degree of parallelism
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = _maxDegreeOfParallelism
            };

            // Create a thread-safe wrapper for the result receiver to avoid race conditions
            var syncResultReceiver = new SynchronizedResultReceiver(resultReceiver_);

            // Process trades until the queue is empty
            Parallel.For(0, _maxDegreeOfParallelism, options, (i) =>
            {
                // Console.WriteLine($"Worker {i} starting on thread ID: {Thread.CurrentThread.ManagedThreadId}");

                while (tradesToProcess.TryDequeue(out ITrade? trade))
                {
                    // Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} processing trade: {trade.TradeId}, Type: {trade.TradeType}");
                    ProcessTrade(trade, syncResultReceiver);
                }
            });
        }

        // Process a single trade using the appropriate pricing engine
        private void ProcessTrade(ITrade trade, IScalarResultReceiver resultReceiver)
        {
            if (!_pricers.ContainsKey(trade.TradeType))
            {
                resultReceiver.AddError(trade.TradeId, "No Pricing Engines available for this trade type");
                return;
            }

            var pricer = _pricers[trade.TradeType];
            pricer.Price(trade, resultReceiver);
        }

        // Loads all the pricing engines from configuration
        private void LoadPricers()
        {
            var pricingConfigLoader = new PricingConfigLoader { ConfigFile = @".\PricingConfig\PricingEngines.xml" };
            var pricerConfig = pricingConfigLoader.LoadConfig();

            foreach (var configItem in pricerConfig)
            {
                if (string.IsNullOrEmpty(configItem.TradeType))
                {
                    throw new InvalidOperationException("Trade type not specified in config");
                }

                if (string.IsNullOrEmpty(configItem.Assembly) || string.IsNullOrEmpty(configItem.TypeName))
                {
                    throw new InvalidOperationException($"Assembly or pricing engine not specified for trade type: {configItem.TradeType}");
                }

                try
                {

                    var shortAssembly = configItem.Assembly.Split('.').Last();
                    var shortAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{shortAssembly}.dll");

                    Assembly assembly = Assembly.LoadFrom(shortAssemblyPath);

                    // Get the type from the assembly
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

        // Thread-safe wrapper for IScalarResultReceiver to ensure 
        // thread safety when adding results from multiple threads
        private class SynchronizedResultReceiver : IScalarResultReceiver
        {
            private readonly IScalarResultReceiver _innerReceiver;
            private readonly object _lock = new object();

            public SynchronizedResultReceiver(IScalarResultReceiver innerReceiver)
            {
                _innerReceiver = innerReceiver;
            }

            public void AddResult(string tradeId_, double result_)
            {
                lock (_lock)
                {
                    _innerReceiver.AddResult(tradeId_, result_);
                }
            }

            public void AddError(string tradeId_, string error_)
            {
                lock (_lock)
                {
                    _innerReceiver.AddError(tradeId_, error_);
                }
            }
        }
    }
}
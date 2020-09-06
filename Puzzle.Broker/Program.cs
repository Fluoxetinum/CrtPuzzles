using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Puzzle.Broker.Infrastructure;
using Puzzle.Broker.Interfaces;
using Puzzle.Common;

[assembly: InternalsVisibleTo("Puzzle.Broker.Tests")]

namespace Puzzle.Broker
{
	class Program
	{
		private static readonly string _userInputTemplate =
			"Puzzle.Broker [consumers_count] [producers_count]";

		private static int _queueCapacity = 10;
		private static SimulationManager _simulationManager;
		private static ISimpleLogger _logger;


		static void Main(string[] args)
		{
			_logger = new SimpleConsoleLogger();
			try
			{
				if (args.Length < 2)
				{
					_logger.Info($"Program input format : '{_userInputTemplate}'");
					return;
				}

				int consumersCount = 0;
				int producersCount = 0;
				bool consumersCountResult = int.TryParse(args[0], out consumersCount);
				bool producersCountResult = int.TryParse(args[1], out producersCount);

				if (!consumersCountResult || !producersCountResult)
				{
					_logger.Info($"Program input format : '{_userInputTemplate}'");
					return;
				}

				List<string> topics = new List<string>()
				{
					"Topic 1",
					"Topic 2",
					"Topic 3"
				};
				TimeSpan producingInterval = TimeSpan.FromMilliseconds(1000);
				ISubscriberCreator consumerCreator = new SubscriberCreator();
				IProducerCreator producerCreator = new ProducerCreator();
				IBrokerManager brokerManager = new BrokerManager(_queueCapacity);

				_simulationManager = new SimulationManager(_logger, producingInterval, topics, brokerManager);
				Console.CancelKeyPress += ConsoleOnCancelKeyPress;
				_logger.Info("Starting simulation... (Press Ctrl+C to stop program).");
				_simulationManager.Start(consumersCount, producersCount, consumerCreator, producerCreator);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Unexpected error");
			}
		}


		private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			_logger.Info("Stopping simulation...");
			((IDisposable)_simulationManager).Dispose();
		}
	}
}

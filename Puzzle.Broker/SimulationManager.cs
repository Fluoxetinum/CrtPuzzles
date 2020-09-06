using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Puzzle.Broker.Infrastructure;
using Puzzle.Broker.Interfaces;
using Puzzle.Common;

namespace Puzzle.Broker
{
	public class SimulationManager : IDisposable
	{
		private static AutoResetEvent _stopEvent;
		private readonly ISimpleLogger _logger;
		private readonly IBrokerManager _brokerManager;
		private readonly List<IProducer> _producers;
		private readonly List<string> _topics;
		private readonly TimeSpan _producingInterval;


		public SimulationManager(
			ISimpleLogger logger,
			TimeSpan producingInterval,
			List<string> topics,
			IBrokerManager brokerManager)
		{
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			if (topics == null)
			{
				throw new ArgumentNullException(nameof(topics));
			}

			if (brokerManager == null)
			{
				throw new ArgumentNullException(nameof(brokerManager));
			}

			_logger = logger;
			_producingInterval = producingInterval;
			_stopEvent = new AutoResetEvent(initialState: false);
			_brokerManager = brokerManager;
			_producers = new List<IProducer>();
			_topics = topics;
		}


		public void Start(
			int consumersCount,
			int producersCount,
			ISubscriberCreator consumerCreator,
			IProducerCreator producerCreator)
		{
			if (consumerCreator == null)
			{
				throw new ArgumentNullException(nameof(consumerCreator));
			}
			if (producerCreator == null)
			{
				throw new ArgumentNullException(nameof(producerCreator));
			}

			ISubscribable subscribable = _brokerManager.GetSubscribable();
			for (int i = 0; i < consumersCount; i++)
			{
				var topicsList = new List<string>();
				string topic1 = _topics[i % _topics.Count];
				string topic2 = _topics[(i + 1) % _topics.Count];
				topicsList.Add(topic1);
				topicsList.Add(topic2);
				ISubscriber subscriber = consumerCreator.Create(_logger, topicsList);
				subscribable.Subscribe(subscriber);
				_logger.Info($"Subscriber#{subscriber.Id} for topics '{topic1}' and '{topic2}' created.");
			}

			IPostable postable = _brokerManager.GetPostable();
			for (int i = 0; i < producersCount; i++)
			{
				var topicsList = new List<string>();
				string topic1 = _topics[i % _topics.Count];
				string topic2 = _topics[(i + 1) % _topics.Count];
				topicsList.Add(topic1);
				topicsList.Add(topic2);
				IProducer producer = producerCreator.Create(postable, _logger, topicsList);
				_producers.Add(producer);
				_logger.Info($"Producer#{producer.Id} for topics '{topic1}' and '{topic2}' created.");
			}

			foreach (var producer in _producers)
			{
				producer.StartProducing(_producingInterval);
			}

			_logger.Info("Broker simulation started.");
			_stopEvent.WaitOne();
			_logger.Info("Broker simulation stopped.");
		}


		public void Stop()
		{
			foreach (var producer in _producers)
			{
				producer.Dispose();
			}
			_stopEvent.Set();
		}


		void IDisposable.Dispose()
		{
			Stop();
			((IDisposable)_brokerManager)?.Dispose();
		}
	}
}

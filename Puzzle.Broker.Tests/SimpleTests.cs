using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Puzzle.Broker.Infrastructure;
using Puzzle.Broker.Interfaces;
using Puzzle.Broker.Tests.Infrastructure;
using Puzzle.Common;
using Xunit;

namespace Puzzle.Broker.Tests
{
	public class SimpleTests
	{
		private volatile int _dequeued;


		[Fact]
		public void ConcurrentQueueTest()
		{
			_dequeued = 0;

			BrokerQueue<string> queue =
				new BrokerQueue<string>(capacity: 5);

			ConcurrentQueue<string> initialQueue = new ConcurrentQueue<string>();
			ConcurrentQueue<string> checkQueue = new ConcurrentQueue<string>();

			int producersCount = 7;
			int messagesByProducer = 10;
			int totalMessages = producersCount * messagesByProducer;
			int consumersCount = 5;

			Thread[] producers = new Thread[producersCount];
			Thread[] consumers = new Thread[consumersCount];

			for (int i = 0; i < consumers.Length; i++)
			{
				consumers[i] = new Thread(() =>
				{
					string msg;
					while (_dequeued < totalMessages)
					{
						bool result = queue.TryDequeue(TimeSpan.FromSeconds(1), out msg);
						if (result)
						{
							checkQueue.Enqueue(msg);
							Interlocked.Increment(ref _dequeued);
						}
					}
				});
				consumers[i].Start();
			}

			for (int i = 0; i < producers.Length; i++)
			{
				int tempI = i;
				producers[i] = new Thread(() =>
				{
					for (int j = 0; j < messagesByProducer; j++)
					{
						string msg = $"Message{tempI}{j}";
						queue.Enqueue(msg);
						initialQueue.Enqueue(msg);
					}
				});
				producers[i].Start();
			}

			foreach (var consumer in consumers)
			{
				consumer.Join();
			}

			foreach (var producer in producers)
			{
				producer.Join();
			}

			queue.Dispose();

			Assert.NotStrictEqual(initialQueue, checkQueue);
			Assert.Equal(0, queue.Count);
		}


		[Fact]
		public void ConcurrentSubscribersDictionaryTest()
		{
			TopicsDictionary dict =
					new TopicsDictionary();

			int count = 400;

			Thread[] producers = new Thread[count];
			Thread[] consumers = new Thread[count];

			List<TestSubscriber> subscribers = new List<TestSubscriber>();
			for (int i = 0; i < count; i++)
			{
				subscribers.Add(new TestSubscriber());
			}

			for (int i = 0; i < consumers.Length; i++)
			{
				int tempI = i;
				consumers[i] = new Thread(() =>
				{
					while (!dict.Remove($"{tempI}", subscribers[tempI]))
					{
						Thread.Sleep(200);
					}

					while (!dict.Remove($"{tempI + 1}", subscribers[tempI]))
					{
						Thread.Sleep(200);
					}
				});
				consumers[i].Start();
			}

			for (int i = 0; i < producers.Length; i++)
			{
				int tempI = i;
				producers[i] = new Thread(() =>
				{
					dict.Add($"{tempI}", subscribers[tempI]);
					dict.Add($"{tempI + 1}", subscribers[tempI]);
				});
				producers[i].Start();
			}

			foreach (var consumer in consumers)
			{
				consumer.Join();
			}

			foreach (var producer in producers)
			{
				producer.Join();
			}

			Assert.Equal(0, dict.Count);
		}


		[Fact]
		public void ConsistencyTest()
		{
			var logger = new TestLogger();
			TimeSpan producingInterval = TimeSpan.FromMilliseconds(500);

			List<string> topics = new List<string>()
			{
				"Topic 1",
				"Topic 2",
				"Topic 3",
				"Topic 4"
			};

			var consumerCreator = new TestSubscriberCreator();
			var producerCreator = new ProducerCreator();
			var brokerManager = new BrokerManager(queueCapacity: 20);

			SimulationManager simulationManager = new SimulationManager(logger, producingInterval, topics, brokerManager);
			Task.Delay(15_000).ContinueWith((t) =>
			{
				((IDisposable)simulationManager).Dispose();
			});
			simulationManager.Start(consumersCount: 5, producersCount: 5, consumerCreator, producerCreator);
		}


		[Fact]
		public void StressTest()
		{
			var logger = new TestLogger();
			TimeSpan producingInterval = TimeSpan.FromMilliseconds(50);

			List<string> topics = new List<string>()
			{
				"Topic 1",
				"Topic 2",
				"Topic 3",
				"Topic 4",
				"Topic 5"
			};

			var consumerCreator = new SubscriberCreator();
			var producerCreator = new ProducerCreator();
			var brokerManager = new BrokerManager(queueCapacity: 50);

			SimulationManager simulationManager = new SimulationManager(logger, producingInterval, topics, brokerManager);
			Task.Delay(15_000).ContinueWith((t) =>
			{
				((IDisposable)simulationManager).Dispose();
			});
			simulationManager.Start(consumersCount: 4, producersCount: 4, consumerCreator, producerCreator);
		}


		[Fact]
		public void NullArgumentsTest()
		{
			ISubscriberCreator consumerCreator = new SubscriberCreator();
			IProducerCreator producerCreator = new ProducerCreator();
			ISimpleLogger logger = new TestLogger();
			List<string> topics = new List<string>();
			Broker b = new Broker(new BrokerQueue<IMessage>(), new TopicsDictionary());
			IBrokerManager manager = new BrokerManager(queueCapacity:10);
			SimulationManager simulationManager = 
				new SimulationManager(logger, TimeSpan.Zero, topics, manager);

			Assert.Throws<ArgumentNullException>(() =>
			{
				consumerCreator.Create(null, topics);
			});
			
			Assert.Throws<ArgumentNullException>(() =>
			{
				consumerCreator.Create(logger, null);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				producerCreator.Create(null, logger, topics);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				var b = new Broker(null, new TopicsDictionary());
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				var b = new Broker(new BrokerQueue<IMessage>(), null);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				producerCreator.Create(b, null, topics);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				producerCreator.Create(b, logger, null);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				var m = new SimulationManager(logger, TimeSpan.Zero, topics, null);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				var m = new SimulationManager(logger, TimeSpan.Zero, null, manager);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				var m = new SimulationManager(null, TimeSpan.Zero, topics, manager);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				simulationManager.Start(0, 0, null, producerCreator);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				simulationManager.Start(0, 0, consumerCreator, null);
			});
		}
	}
}
